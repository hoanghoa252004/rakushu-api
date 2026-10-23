using System.Text.Json;
using Microsoft.Extensions.Logging;
using Rakushu.Application.Abstractions.Infrastructure;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Usecases.Learning.Srs.Common;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs;

namespace Rakushu.Application.Usecases.Learning.Srs.SubmitQuizAttempt;

internal interface IQuizAttemptFeedbackService
{
    Task<QuizAttemptResultResponse> ApplyAsync(
        QuizAttemptResultResponse submissionResult,
        CancellationToken cancellationToken);
}

internal sealed class QuizAttemptFeedbackService : IQuizAttemptFeedbackService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly ILogger<QuizAttemptFeedbackService> _logger;
    private readonly ICurrentUserContext _currentUser;
    private readonly IQuizContentGenerator _quizContentGenerator;
    private readonly IQuizAttemptRepository _attemptRepository;
    private readonly IUnitOfWork _unitOfWork;

    public QuizAttemptFeedbackService(
        ILogger<QuizAttemptFeedbackService> logger,
        ICurrentUserContext currentUser,
        IQuizContentGenerator quizContentGenerator,
        IQuizAttemptRepository attemptRepository,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _currentUser = currentUser;
        _quizContentGenerator = quizContentGenerator;
        _attemptRepository = attemptRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<QuizAttemptResultResponse> ApplyAsync(
        QuizAttemptResultResponse submissionResult,
        CancellationToken cancellationToken)
    {
        var feedbackAttempt = await _attemptRepository.GetForUserAsync(
            QuizAttemptId.From(submissionResult.AttemptId),
            _currentUser.UserId!,
            cancellationToken);
        var feedbackItemsById = feedbackAttempt?.Items.ToDictionary(item => item.Id.Value);
        IReadOnlyDictionary<Guid, QuizCardAnswerHistory> answerHistoryByCardId =
            new Dictionary<Guid, QuizCardAnswerHistory>();

        if (feedbackAttempt is not null && _currentUser.UserId is { } feedbackUserId)
        {
            var cardIds = feedbackAttempt.Items.Select(item => item.SrsCardId.Value).Distinct().ToArray();
            try
            {
                answerHistoryByCardId = await _attemptRepository.GetAnswerHistoryByCardAsync(
                    feedbackUserId,
                    cardIds,
                    QuizAttemptId.From(feedbackAttempt.Id.Value),
                    cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception,
                    "Could not load answer-history context for quiz attempt {AttemptId}; fallback feedback will be used.",
                    feedbackAttempt.Id.Value);
            }
        }

        var feedbackPrompts = submissionResult.Results.Select(question =>
        {
            QuizAttemptItem? feedbackItem = null;
            var hasFeedbackItem = feedbackItemsById is not null &&
                feedbackItemsById.TryGetValue(question.ItemId, out feedbackItem);
            var cardId = hasFeedbackItem ? feedbackItem!.SrsCardId.Value : Guid.Empty;
            var history = answerHistoryByCardId.TryGetValue(cardId, out var cardHistory)
                ? cardHistory
                : new QuizCardAnswerHistory(0, 0, 0);

            return new QuizFeedbackPrompt(
                question.ItemId,
                question.IsCorrect,
                history.TotalAttempts,
                history.IncorrectAttempts);
        }).ToList();

        IReadOnlyDictionary<Guid, string> adaptiveFeedback;
        try
        {
            adaptiveFeedback = await _quizContentGenerator.GenerateAdaptiveFeedbackAsync(
                feedbackPrompts, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception,
                "Adaptive feedback generation failed for quiz attempt {AttemptId}; fallback feedback will be used.",
                submissionResult.AttemptId);
            adaptiveFeedback = new Dictionary<Guid, string>();
        }

        var feedbackPromptsByItemId = feedbackPrompts.ToDictionary(prompt => prompt.ItemId);
        var updatedResults = submissionResult.Results
            .Select(question => question with
            {
                Feedback = ResolveFeedback(feedbackPromptsByItemId[question.ItemId], adaptiveFeedback)
            })
            .ToList();

        try
        {
            await _unitOfWork.ExecuteAsync(async () =>
            {
                var userId = _currentUser.UserId;
                if (userId is null)
                {
                    return Result.Failure(UserErrors.UnauthorizedResourceAccess);
                }

                var attempt = await _attemptRepository.GetForUserAsync(
                    QuizAttemptId.From(submissionResult.AttemptId),
                    userId,
                    cancellationToken);
                if (attempt is null)
                {
                    return Result.Failure(QuizAttemptErrors.QuizAttemptNotFound);
                }

                foreach (var item in attempt.Items)
                {
                    var feedback = ResolveFeedback(
                        feedbackPromptsByItemId[item.Id.Value],
                        adaptiveFeedback);
                    item.UpdateFeedbackSnapshot(JsonSerializer.Serialize(
                        new { adaptive = feedback }, JsonOptions));
                }

                return Result.Success();
            }, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            // Submission and SM-2 changes are already committed; keep returning the graded result.
            _logger.LogWarning(exception,
                "Could not persist adaptive feedback for quiz attempt {AttemptId}; returning the graded result.",
                submissionResult.AttemptId);
        }

        return submissionResult with { Results = updatedResults };
    }

    private static string BuildFallbackFeedback(QuizFeedbackPrompt prompt)
    {
        if (prompt.IsCorrect)
        {
            if (prompt.PreviousIncorrectCount > 0)
                return "Bạn đã trả lời đúng từ này sau những lần nhầm trước đó. Hãy tiếp tục ôn để củng cố kết quả.";

            return prompt.PreviousAttemptCount > 0
                ? "Bạn tiếp tục nhớ đúng từ này. Hãy duy trì nhịp ôn tập để giữ kiến thức lâu dài."
                : "Bạn đã nhớ đúng ngay lần đầu. Đây là khởi đầu tốt cho việc ghi nhớ.";
        }

        if (prompt.PreviousIncorrectCount >= 2)
            return "Từ này vẫn còn dễ nhầm với bạn. Hãy ôn lại nghĩa và thử ghi nhớ chủ động trước lần kiểm tra tiếp theo.";
        if (prompt.PreviousIncorrectCount == 1)
            return "Bạn đã nhầm từ này trước đây; hãy dành thêm một lượt ôn ngắn để củng cố trí nhớ.";

        return prompt.PreviousAttemptCount > 0
            ? "Lần này bạn chưa nhớ đúng từ đã gặp trước đó. Hãy ôn lại và thử truy xuất từ trí nhớ lần nữa."
            : "Bạn chưa nhớ đúng từ này lần đầu. Đừng lo, việc ôn lại sẽ giúp củng cố trí nhớ.";
    }

    private static string ResolveFeedback(
        QuizFeedbackPrompt prompt,
        IReadOnlyDictionary<Guid, string> adaptiveFeedback)
    {
        return adaptiveFeedback.TryGetValue(prompt.ItemId, out var message) &&
            !string.IsNullOrWhiteSpace(message) &&
            message.Length <= 180
                ? message.Trim()
                : BuildFallbackFeedback(prompt);
    }
}
