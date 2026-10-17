using System.Text.Json;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Usecases.Learning.Srs.Common;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs;

namespace Rakushu.Application.Usecases.Learning.Srs.SubmitQuizAttempt;

internal sealed class SubmitQuizAttemptHandler
    : IRequestHandler<SubmitQuizAttemptCommand, Result<QuizAttemptResultResponse>>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private static bool TryGetPropertyIgnoreCase(
        JsonElement element,
        string propertyName,
        out JsonElement value)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in element.EnumerateObject())
            {
                if (string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase))
                {
                    value = property.Value;
                    return true;
                }
            }
        }

        value = default;
        return false;
    }

    private readonly ICurrentUserContext _currentUser;
    private readonly IQuizAttemptRepository _attemptRepository;
    private readonly ISrsCardRepository _cardRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISystemClock _clock;

    public SubmitQuizAttemptHandler(
        ICurrentUserContext currentUser,
        IQuizAttemptRepository attemptRepository,
        ISrsCardRepository cardRepository,
        IUnitOfWork unitOfWork,
        ISystemClock clock)
    {
        _currentUser = currentUser;
        _attemptRepository = attemptRepository;
        _cardRepository = cardRepository;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public async Task<Result<QuizAttemptResultResponse>> Handle(
        SubmitQuizAttemptCommand request,
        CancellationToken cancellationToken)
    {
        return await _unitOfWork.ExecuteAsync(async () =>
        {
            var userId = _currentUser.UserId;
            if (userId is null)
            {
                return Result.Failure<QuizAttemptResultResponse>(
                    UserErrors.UnauthorizedResourceAccess);
            }

            var attempt = await _attemptRepository.GetForUserAsync(
                QuizAttemptId.From(request.AttemptId),
                userId,
                cancellationToken);

            if (attempt is null)
            {
                return Result.Failure<QuizAttemptResultResponse>(
                    QuizAttemptErrors.QuizAttemptNotFound);
            }

            if (attempt.Status != QuizAttemptStatus.InProgress)
            {
                return Result.Failure<QuizAttemptResultResponse>(
                    QuizAttemptErrors.QuizNotInProgress);
            }

            if (request.Answers is null ||
                request.Answers.Count != attempt.Items.Count ||
                request.Answers.Select(answer => answer.ItemId).Distinct().Count() != request.Answers.Count)
            {
                return Result.Failure<QuizAttemptResultResponse>(
                    QuizAttemptErrors.QuizHasUnansweredItems);
            }

            var itemsById = attempt.Items.ToDictionary(item => item.Id.Value);

            foreach (var answer in request.Answers)
            {
                if (!itemsById.TryGetValue(answer.ItemId, out var item) ||
                    string.IsNullOrWhiteSpace(answer.SelectedOptionId))
                {
                    return Result.Failure<QuizAttemptResultResponse>(
                        QuizAttemptErrors.InvalidQuizAnswer);
                }

                using var choicesDocument = JsonDocument.Parse(item.ChoicesSnapshot ?? "[]");
                var isValidOption = choicesDocument.RootElement
                    .EnumerateArray()
                    .Any(choice =>
                        TryGetPropertyIgnoreCase(choice, "id", out var id) &&
                        id.ValueKind == JsonValueKind.String &&
                        string.Equals(
                            id.GetString(),
                            answer.SelectedOptionId,
                            StringComparison.Ordinal));

                if (!isValidOption)
                {
                    return Result.Failure<QuizAttemptResultResponse>(
                        QuizAttemptErrors.InvalidQuizAnswer);
                }
            }

            var now = _clock.UtcNow;

            foreach (var answer in request.Answers)
            {
                var item = itemsById[answer.ItemId];

                using var correctAnswerDocument = JsonDocument.Parse(item.CorrectAnswerSnapshot);
                var correctOptionId = correctAnswerDocument.RootElement
                    .GetProperty("optionId")
                    .GetString()!;

                var isCorrect = string.Equals(
                    correctOptionId,
                    answer.SelectedOptionId,
                    StringComparison.Ordinal);

                var recordAnswerResult = attempt.RecordAnswer(
                    answer.ItemId,
                    answer.SelectedOptionId,
                    isCorrect,
                    now);

                if (recordAnswerResult.IsFailure)
                {
                    return Result.Failure<QuizAttemptResultResponse>(recordAnswerResult.Error);
                }
            }

            var completeResult = attempt.Complete(now);
            if (completeResult.IsFailure)
            {
                return Result.Failure<QuizAttemptResultResponse>(completeResult.Error);
            }

            var questionResults = new List<QuizQuestionResultDto>();

            foreach (var item in attempt.Items.OrderBy(item => item.Position))
            {
                using var questionDocument = JsonDocument.Parse(item.QuestionSnapshot);
                using var correctAnswerDocument = JsonDocument.Parse(item.CorrectAnswerSnapshot);
                using var choicesDocument = JsonDocument.Parse(item.ChoicesSnapshot ?? "[]");
                using var feedbackDocument = JsonDocument.Parse(item.FeedbackSnapshot ?? "{}");

                var correctOptionId = correctAnswerDocument.RootElement
                    .GetProperty("optionId")
                    .GetString()!;

                var prompt = questionDocument.RootElement
                    .GetProperty("prompt")
                    .GetString() ?? string.Empty;

                var reading = questionDocument.RootElement.TryGetProperty("reading", out var readingElement)
                    ? readingElement.GetString()
                    : null;

                var choices = JsonSerializer.Deserialize<List<QuizChoiceDto>>(
                    choicesDocument.RootElement.GetRawText(),
                    JsonOptions) ?? [];

                var feedbackKey = item.IsCorrect == true ? "correct" : "incorrect";
                var feedback = feedbackDocument.RootElement.TryGetProperty(feedbackKey, out var feedbackElement)
                    ? feedbackElement.GetString() ?? string.Empty
                    : string.Empty;

                if (attempt.AttemptMode == "SrsReview")
                {
                    var card = item.SrsCard;
                    var state = card.State;

                    if (state is null)
                    {
                        return Result.Failure<QuizAttemptResultResponse>(
                            SrsCardErrors.CardStateMissing);
                    }

                    var previousDueAt = state.DueAt;
                    var previousIntervalDays = state.IntervalDays;
                    var previousEaseFactor = state.EasinessFactor;
                    var previousRepetitions = state.Repetitions;

                    var rating = item.IsCorrect == true ? "Good" : "Again";
                    var reviewResult = card.RecordReview(rating, now);

                    if (reviewResult.IsFailure)
                    {
                        return Result.Failure<QuizAttemptResultResponse>(reviewResult.Error);
                    }

                    var reviewLog = SrsReviewLog.Create(
                        card.Id,
                        null,
                        item.Id,
                        rating,
                        now,
                        null,
                        previousDueAt,
                        state.DueAt,
                        previousIntervalDays,
                        state.IntervalDays,
                        previousEaseFactor,
                        state.EasinessFactor,
                        previousRepetitions,
                        state.Repetitions,
                        now);

                    _cardRepository.AddReviewLog(reviewLog);
                }

                questionResults.Add(new QuizQuestionResultDto(
                    item.Id.Value,
                    item.Position,
                    prompt,
                    reading,
                    item.SelectedAnswer!,
                    correctOptionId,
                    item.IsCorrect == true,
                    feedback,
                    choices));
            }

            return Result.Success(new QuizAttemptResultResponse(
                attempt.Id.Value,
                attempt.Status.ToString(),
                (int)Math.Round(attempt.Score ?? 0m),
                attempt.TotalItems,
                attempt.CorrectItems,
                attempt.CompletedAt!.Value,
                questionResults));
        }, cancellationToken);
    }
}