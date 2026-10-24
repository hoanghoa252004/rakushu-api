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

	// CONTEXT & CLOCK
	private readonly ICurrentUserContext _currentUser;
	private readonly ISystemClock _clock;

	// REPOSITORIES & UOW
	private readonly IQuizAttemptRepository _attemptRepository;
	private readonly IUnitOfWork _unitOfWork;

	// SERVICES
	private readonly IQuizAttemptFeedbackService _feedbackService;

	public SubmitQuizAttemptHandler(
		ICurrentUserContext currentUser,
		IQuizAttemptRepository attemptRepository,
		IQuizAttemptFeedbackService feedbackService,
		IUnitOfWork unitOfWork,
		ISystemClock clock)
	{
		_currentUser = currentUser;
		_attemptRepository = attemptRepository;
		_feedbackService = feedbackService;
		_unitOfWork = unitOfWork;
		_clock = clock;
	}

	public async Task<Result<QuizAttemptResultResponse>> Handle(
		SubmitQuizAttemptCommand request,
		CancellationToken cancellationToken)
	{
		// 1. Validate User Context
		var userId = _currentUser.UserId;
		if (userId is null)
		{
			return Result.Failure<QuizAttemptResultResponse>(
				UserErrors.UnauthorizedResourceAccess);
		}

		var answers = request.Answers;
		if (answers is null ||
			answers.Count == 0 ||
			answers.Any(answer => answer.ItemId == Guid.Empty || string.IsNullOrWhiteSpace(answer.SelectedOptionId)) ||
			answers.Select(answer => answer.ItemId).Distinct().Count() != answers.Count)
		{
			return Result.Failure<QuizAttemptResultResponse>(
				QuizAttemptErrors.InvalidQuizAnswer);
		}

		var now = _clock.UtcNow;

		// 2. Execute Review Submission in Transaction
		var submissionResult = await _unitOfWork.ExecuteAsync(async () =>
		{
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

			if (attempt.Items.Count != answers.Count)
			{
				return Result.Failure<QuizAttemptResultResponse>(
					QuizAttemptErrors.InvalidQuizAnswer);
			}

			var answersByItemId = answers.ToDictionary(answer => answer.ItemId);
			if (!attempt.Items.All(item => answersByItemId.ContainsKey(item.Id.Value)))
			{
				return Result.Failure<QuizAttemptResultResponse>(
					QuizAttemptErrors.InvalidQuizAnswer);
			}

			// 3. Record Answer for Each Item
			foreach (var item in attempt.Items)
			{
				var answer = answersByItemId[item.Id.Value];
				var question = item.Question;
				var isCorrect = string.Equals(
					answer.SelectedOptionId,
					question.CorrectOptionId,
					StringComparison.OrdinalIgnoreCase);

				var elapsedMs = QuizAnswerQualityScorer.NormalizeElapsedMs(answer.ElapsedMs);
				var quality = QuizAnswerQualityScorer.Calculate(
					isCorrect,
					elapsedMs,
					!string.IsNullOrWhiteSpace(answer.SelectedOptionId));

				var answerResult = attempt.RecordAnswer(
					item.Id.Value,
					answer.SelectedOptionId,
					isCorrect,
					elapsedMs ?? 0,
					quality,
					null,
					now);

				if (answerResult.IsFailure)
				{
					return Result.Failure<QuizAttemptResultResponse>(answerResult.Error);
				}
			}

			// 4. Mark Quiz Attempt as Completed
			var completeResult = attempt.Complete(now);
			if (completeResult.IsFailure)
			{
				return Result.Failure<QuizAttemptResultResponse>(completeResult.Error);
			}

			// 5. Build Initial Result & Update SM-2 Metrics
			var builderResult = QuizAttemptResultBuilder.Build(
				attempt,
				answersByItemId,
				now);

			if (builderResult.IsFailure)
			{
				return Result.Failure<QuizAttemptResultResponse>(builderResult.Error);
			}

			return Result.Success(builderResult.Value);
		}, cancellationToken);

		if (submissionResult.IsFailure)
		{
			return submissionResult;
		}

		// 6. Enrich with Adaptive Feedback asynchronously
		var resultWithFeedback = await _feedbackService.ApplyAsync(
			submissionResult.Value,
			cancellationToken);

		return Result.Success(resultWithFeedback);
	}
}
