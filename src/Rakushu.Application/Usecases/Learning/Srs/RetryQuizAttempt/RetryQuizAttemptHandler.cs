using System.Text.Json;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Usecases.Learning.Srs.Common;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs;

namespace Rakushu.Application.Usecases.Learning.Srs.RetryQuizAttempt;

internal sealed class RetryQuizAttemptHandler
	: IRequestHandler<RetryQuizAttemptCommand, Result<QuizAttemptStartedResponse>>
{
	private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

	private readonly ICurrentUserContext _currentUser;
	private readonly IQuizAttemptRepository _attemptRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ISystemClock _clock;

	public RetryQuizAttemptHandler(
		ICurrentUserContext currentUser,
		IQuizAttemptRepository attemptRepository,
		IUnitOfWork unitOfWork,
		ISystemClock clock)
	{
		_currentUser = currentUser;
		_attemptRepository = attemptRepository;
		_unitOfWork = unitOfWork;
		_clock = clock;
	}

	public async Task<Result<QuizAttemptStartedResponse>> Handle(
		RetryQuizAttemptCommand request,
		CancellationToken cancellationToken)
	{
		// 1. Validate User Context
		var userId = _currentUser.UserId;
		if (userId is null)
		{
			return Result.Failure<QuizAttemptStartedResponse>(
				UserErrors.UnauthorizedResourceAccess);
		}

		// 2. Validate Retry Mode
		var retryModeInput = (request.RetryMode ?? string.Empty).Trim();
		QuizAttemptMode targetMode;
		if (string.Equals(retryModeInput, "All", StringComparison.OrdinalIgnoreCase))
		{
			targetMode = QuizAttemptMode.PracticeAll;
		}
		else if (string.Equals(retryModeInput, "MistakesOnly", StringComparison.OrdinalIgnoreCase))
		{
			targetMode = QuizAttemptMode.PracticeMistakes;
		}
		else
		{
			return Result.Failure<QuizAttemptStartedResponse>(QuizAttemptErrors.InvalidRetryMode);
		}

		// 3. Fetch Previous Completed Attempt
		var previousAttempt = await _attemptRepository.GetForUserAsync(
			QuizAttemptId.From(request.AttemptId),
			userId,
			cancellationToken);

		if (previousAttempt is null)
		{
			return Result.Failure<QuizAttemptStartedResponse>(QuizAttemptErrors.QuizAttemptNotFound);
		}

		if (previousAttempt.Status != QuizAttemptStatus.Completed)
		{
			return Result.Failure<QuizAttemptStartedResponse>(QuizAttemptErrors.AttemptNotCompleted);
		}

		// 4. Select Items to Retry
		var sourceItems = previousAttempt.Items.OrderBy(item => item.Position).ToList();
		List<QuizAttemptItem> itemsToRetry;

		if (targetMode == QuizAttemptMode.PracticeMistakes)
		{
			itemsToRetry = sourceItems.Where(item => item.IsCorrect == false).ToList();
			if (itemsToRetry.Count == 0)
			{
				return Result.Failure<QuizAttemptStartedResponse>(QuizAttemptErrors.NoMistakesToRetry);
			}
		}
		else
		{
			itemsToRetry = sourceItems;
		}

		var now = _clock.UtcNow;

		// 5. Create New Attempt in Transaction
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var attemptResult = QuizAttempt.Create(
				userId,
				previousAttempt.QuizId,
				now,
				targetMode);

			if (attemptResult.IsFailure)
			{
				return Result.Failure<QuizAttemptStartedResponse>(attemptResult.Error);
			}

			var newAttempt = attemptResult.Value;
			var responseQuestions = new List<QuizQuestionDto>();

			for (var index = 0; index < itemsToRetry.Count; index++)
			{
				var sourceItem = itemsToRetry[index];
				var question = sourceItem.Question;
				var position = index + 1;

				var newItemResult = QuizAttemptItem.Create(
					sourceItem.QuestionId,
					sourceItem.SrsItemId,
					position,
					now);

				if (newItemResult.IsFailure)
				{
					return Result.Failure<QuizAttemptStartedResponse>(newItemResult.Error);
				}

				var addItemResult = newAttempt.AddItem(newItemResult.Value);
				if (addItemResult.IsFailure)
				{
					return Result.Failure<QuizAttemptStartedResponse>(addItemResult.Error);
				}

				var choices = JsonSerializer.Deserialize<List<QuizChoiceDto>>(
					question.ChoicesJson,
					JsonOptions) ?? [];

				responseQuestions.Add(new QuizQuestionDto(
					newItemResult.Value.Id.Value,
					position,
					question.QuestionType,
					question.Prompt,
					question.Reading,
					choices));
			}

			_attemptRepository.Add(newAttempt);

			return Result.Success(new QuizAttemptStartedResponse(
				newAttempt.Id.Value,
				newAttempt.Status.ToString(),
				newAttempt.Mode.ToString(),
				newAttempt.StartedAt,
				responseQuestions));
		}, cancellationToken);
	}
}
