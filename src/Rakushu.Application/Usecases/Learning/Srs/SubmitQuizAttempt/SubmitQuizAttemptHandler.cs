using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using Rakushu.Application.Abstractions.Infrastructure;
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

	private readonly ILogger<SubmitQuizAttemptHandler> _logger;
	private readonly ICurrentUserContext _currentUser;
	private readonly IQuizContentGenerator _quizContentGenerator;
	private readonly IQuizAttemptRepository _attemptRepository;
	private readonly ISrsCardRepository _cardRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ISystemClock _clock;

	public SubmitQuizAttemptHandler(
		ILogger<SubmitQuizAttemptHandler> logger,
		ICurrentUserContext currentUser,
		IQuizContentGenerator quizContentGenerator,
		IQuizAttemptRepository attemptRepository,
		ISrsCardRepository cardRepository,
		IUnitOfWork unitOfWork,
		ISystemClock clock)
	{
		_logger = logger;
		_currentUser = currentUser;
		_quizContentGenerator = quizContentGenerator;
		_attemptRepository = attemptRepository;
		_cardRepository = cardRepository;
		_unitOfWork = unitOfWork;
		_clock = clock;
	}

	private static string BuildFallbackFeedback(bool isCorrect, bool hasPreviousAttempt)
	{
		if (isCorrect)
		{
			return hasPreviousAttempt
				? "Bạn đã nhớ đúng từ này ở lần ôn lại. Khả năng ghi nhớ đang được củng cố."
				: "Bạn đã nhớ đúng ngay lần đầu. Đây là khởi đầu tốt cho việc ghi nhớ.";
		}

		return hasPreviousAttempt
			? "Bạn chưa nhớ chính xác từ này ở lần ôn lại. Hãy tiếp tục ôn để củng cố trí nhớ."
			: "Bạn chưa nhớ đúng từ này lần đầu. Đừng lo, việc ôn lại sẽ giúp củng cố trí nhớ.";
	}
	private static string ResolveFeedback(
		Guid itemId,
		bool isCorrect,
		IReadOnlyDictionary<Guid, bool> hasPreviousAttemptByItemId,
		IReadOnlyDictionary<Guid, string> adaptiveFeedback)
	{
		var hasPreviousAttempt = hasPreviousAttemptByItemId.TryGetValue(itemId, out var attemptedBefore) && attemptedBefore;
		return adaptiveFeedback.TryGetValue(itemId, out var message) &&
			!string.IsNullOrWhiteSpace(message) &&
			message.Length <= 180
				? message.Trim()
				: BuildFallbackFeedback(isCorrect, hasPreviousAttempt);
	}

	public async Task<Result<QuizAttemptResultResponse>> Handle(
		SubmitQuizAttemptCommand request,
		CancellationToken cancellationToken)
	{
		var submissionResult = await _unitOfWork.ExecuteAsync(async () =>
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

			if (request.Answers.Any(answer => answer.ElapsedMs is < 0))
			{
				return Result.Failure<QuizAttemptResultResponse>(
					QuizAttemptErrors.InvalidQuizAnswer);
			}

			var itemsById = attempt.Items.ToDictionary(item => item.Id.Value);
			var answersByItemId = request.Answers.ToDictionary(answer => answer.ItemId);

			foreach (var answer in request.Answers)
			{
				if (!itemsById.TryGetValue(answer.ItemId, out var item))
				{
					return Result.Failure<QuizAttemptResultResponse>(
						QuizAttemptErrors.InvalidQuizAnswer);
				}

				if (string.IsNullOrWhiteSpace(answer.SelectedOptionId))
					continue;

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
					answer.SelectedOptionId ?? string.Empty,
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

				var feedback = feedbackDocument.RootElement.TryGetProperty("adaptive", out var adaptiveElement)
					? adaptiveElement.GetString() ?? string.Empty
					: feedbackDocument.RootElement.TryGetProperty(item.IsCorrect == true ? "correct" : "incorrect", out var feedbackElement)
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

					var submittedElapsedMs = answersByItemId[item.Id.Value].ElapsedMs;
					var elapsedMs = QuizAnswerQualityScorer.NormalizeElapsedMs(submittedElapsedMs);
					var quality = QuizAnswerQualityScorer.Calculate(
						item.IsCorrect == true,
						elapsedMs,
						!string.IsNullOrWhiteSpace(item.SelectedAnswer));
					var rating = SrsReviewRating.FromQuality(quality);
					var reviewResult = card.RecordReview(quality, now);

					if (reviewResult.IsFailure)
					{
						return Result.Failure<QuizAttemptResultResponse>(reviewResult.Error);
					}

					var reviewLog = SrsReviewLog.Create(
						card.Id,
						null,
						item.Id,
						rating,
						quality,
						now,
						elapsedMs,
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
					item.QuestionType,
					prompt,
					reading,
					item.SelectedAnswer ?? string.Empty,
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

		if (submissionResult.IsFailure)
		{
			return submissionResult;
		}

		var feedbackAttempt = await _attemptRepository.GetForUserAsync(
			QuizAttemptId.From(submissionResult.Value.AttemptId),
			_currentUser.UserId!,
			cancellationToken);
		var feedbackItemsById = feedbackAttempt?.Items.ToDictionary(item => item.Id.Value);
		IReadOnlySet<Guid> cardsWithPreviousAnswers = new HashSet<Guid>();
		if (feedbackAttempt is not null && _currentUser.UserId is { } feedbackUserId)
		{
			var cardIds = feedbackAttempt.Items.Select(item => item.SrsCardId.Value).Distinct().ToArray();
			try
			{
				cardsWithPreviousAnswers = await _attemptRepository.GetCardsWithPreviousAnswersAsync(
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
					"Could not load previous-answer context for quiz attempt {AttemptId}; fallback feedback will be used.",
					feedbackAttempt.Id.Value);
			}
		}

		var feedbackPrompts = submissionResult.Value.Results.Select(question =>
		{
			QuizAttemptItem? feedbackItem = null;
			var hasFeedbackItem = feedbackItemsById is not null && feedbackItemsById.TryGetValue(question.ItemId, out feedbackItem);
			var cardId = hasFeedbackItem ? feedbackItem!.SrsCardId.Value : Guid.Empty;
			return new QuizFeedbackPrompt(
				question.ItemId,
				question.IsCorrect,
				cardsWithPreviousAnswers.Contains(cardId));
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
				submissionResult.Value.AttemptId);
			adaptiveFeedback = new Dictionary<Guid, string>();
		}

		var hasPreviousAttemptByItemId = feedbackPrompts.ToDictionary(
			prompt => prompt.ItemId,
			prompt => prompt.HasPreviousAttempt);
		var updatedResults = submissionResult.Value.Results
			.Select(question => question with
			{
				Feedback = ResolveFeedback(
					question.ItemId,
					question.IsCorrect,
					hasPreviousAttemptByItemId,
					adaptiveFeedback)
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
					QuizAttemptId.From(submissionResult.Value.AttemptId),
					userId,
					cancellationToken);
				if (attempt is null)
				{
					return Result.Failure(QuizAttemptErrors.QuizAttemptNotFound);
				}

				foreach (var item in attempt.Items)
				{
					var feedback = ResolveFeedback(
						item.Id.Value,
						item.IsCorrect == true,
						hasPreviousAttemptByItemId,
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
				submissionResult.Value.AttemptId);
		}

		return Result.Success(submissionResult.Value with { Results = updatedResults });
	}
}
