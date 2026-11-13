using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.SubmitQuizAttempt;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.Quiz;
using Rakushu.Domain.Entities.Srs.QuizAttempt;
using Rakushu.Domain.Entities.Srs.QuizAttempt.QuizAttemptItem;

namespace Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.CreateAttemptDetail;

internal sealed class CreateAttemptDetailHandler : IRequestHandler<CreateAttemptDetailCommand, Result<Guid>>
{
	private readonly IQuizAttemptRepository _quizAttemptRepository;
	private readonly IQuizAttemptSrsService _quizAttemptSrsService;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public CreateAttemptDetailHandler(
		IQuizAttemptRepository quizAttemptRepository,
		IQuizAttemptSrsService quizAttemptSrsService,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_quizAttemptRepository = quizAttemptRepository;
		_quizAttemptSrsService = quizAttemptSrsService;
		_currentUserContext = currentUserContext;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<Guid>> Handle(
		CreateAttemptDetailCommand request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
			return Result.Failure<Guid>(UserErrors.NotFound);

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var attemptId = QuizAttemptId.From(request.AttemptId);
			var attempt = await _quizAttemptRepository.GetByIdWithItemsAndQuizAsync(attemptId, cancellationToken);
			if (attempt is null || attempt.UserId != userId)
				return Result.Failure<Guid>(QuizAttemptErrors.NotFound);

			if (attempt.Status == QuizAttemptStatus.Completed)
				return Result.Failure<Guid>(QuizAttemptErrors.AlreadyCompleted);

			if (attempt.Status == QuizAttemptStatus.Canceled)
				return Result.Failure<Guid>(QuizAttemptErrors.AlreadyCanceled);

			if (attempt.Status == QuizAttemptStatus.Expired)
				return Result.Failure<Guid>(QuizAttemptErrors.AttemptExpired);

			if (attempt.Status != QuizAttemptStatus.InProgress)
				return Result.Failure<Guid>(QuizAttemptErrors.NotInProgress);

			var now = _systemClock.UtcNow;
			if (attempt.IsExpired(now))
			{
				attempt.Expire(now);
				if (attempt.HasAnyAnsweredItems)
				{
					await _quizAttemptSrsService.ApplySrsUpdatesAsync(attempt, now, cancellationToken);
				}
				_quizAttemptRepository.Update(attempt);
				return Result.Failure<Guid>(QuizAttemptErrors.AttemptExpired);
			}

			var question = attempt.Quiz.Questions.FirstOrDefault(q => q.Id.Value == request.QuizItemId);
			if (question is null)
				return Result.Failure<Guid>(QuizErrors.NotFound);

			if (attempt.Items.Any(i => i.QuestionId == question.Id))
				return Result.Failure<Guid>(QuizAttemptErrors.QuestionAlreadyAnswered);

			if (string.IsNullOrWhiteSpace(request.SelectedAnswer) || QuizAnswerHelper.IsSkip(request.SelectedAnswer))
				return Result.Failure<Guid>(QuizAttemptItemErrors.InvalidSelectedAnswer);

			var typeCode = question.QuestionType?.Code;
			bool isCorrect = CheckIsCorrect(request.SelectedAnswer, question.CorrectAnswer, question.Answers, typeCode);
			int timeSpentMs = Math.Max(0, request.TimeSpentMs);
			string storedAnswer = request.SelectedAnswer.Trim();

			var itemResult = QuizAttemptItem.Create(
				question.Id,
				storedAnswer,
				isCorrect,
				timeSpentMs,
				feedback: null,
				answeredAt: now);

			if (itemResult.IsFailure)
				return Result.Failure<Guid>(itemResult.Error);

			var item = itemResult.Value;
			var addResult = attempt.AddItem(item);
			if (addResult.IsFailure)
				return Result.Failure<Guid>(addResult.Error);

			_quizAttemptRepository.Update(attempt);
			return Result.Success(item.Id.Value);
		}, cancellationToken);
	}

	private static bool CheckIsCorrect(
		string? userAnswer,
		string correctOptionId,
		string choicesJson,
		string? questionTypeCode)
	{
		if (string.IsNullOrWhiteSpace(userAnswer))
			return false;

		var trimmedUser = userAnswer.Trim();

		if (string.Equals(questionTypeCode, "TYPE_READING", StringComparison.OrdinalIgnoreCase) ||
		    string.Equals(questionTypeCode, "TYPE_MEANING", StringComparison.OrdinalIgnoreCase) ||
		    string.Equals(questionTypeCode, "AUDIO_DICTATION", StringComparison.OrdinalIgnoreCase))
		{
			return string.Equals(trimmedUser, correctOptionId.Trim(), StringComparison.OrdinalIgnoreCase);
		}

		if (string.Equals(trimmedUser, correctOptionId.Trim(), StringComparison.OrdinalIgnoreCase))
			return true;

		var choices = QuizOptionParser.Parse(choicesJson);
		var matchedOption = choices.FirstOrDefault(c =>
			string.Equals(c.Id, correctOptionId, StringComparison.OrdinalIgnoreCase) ||
			string.Equals(c.Text, correctOptionId, StringComparison.OrdinalIgnoreCase));

		if (matchedOption != null)
		{
			if (string.Equals(trimmedUser, matchedOption.Id, StringComparison.OrdinalIgnoreCase))
				return true;
			if (string.Equals(trimmedUser, matchedOption.Text, StringComparison.OrdinalIgnoreCase))
				return true;
		}

		return false;
	}
}
