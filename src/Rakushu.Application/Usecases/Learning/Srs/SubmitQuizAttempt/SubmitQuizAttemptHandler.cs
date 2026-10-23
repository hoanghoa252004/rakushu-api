using System.Text.Json;
using MediatR;
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
	private readonly IQuizAttemptFeedbackService _feedbackService;
	private readonly ISrsCardRepository _cardRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ISystemClock _clock;

	public SubmitQuizAttemptHandler(
		ICurrentUserContext currentUser,
		IQuizAttemptRepository attemptRepository,
		IQuizAttemptFeedbackService feedbackService,
		ISrsCardRepository cardRepository,
		IUnitOfWork unitOfWork,
		ISystemClock clock)
	{
		_currentUser = currentUser;
		_attemptRepository = attemptRepository;
		_feedbackService = feedbackService;
		_cardRepository = cardRepository;
		_unitOfWork = unitOfWork;
		_clock = clock;
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

			return QuizAttemptResultBuilder.Build(
				attempt,
				answersByItemId,
				now,
				_cardRepository);
		}, cancellationToken);

		if (submissionResult.IsFailure)
		{
			return submissionResult;
		}

		var resultWithFeedback = await _feedbackService.ApplyAsync(
			submissionResult.Value,
			cancellationToken);

		return Result.Success(resultWithFeedback);
	}
}
