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

namespace Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.UpdateAttemptDetail;

internal sealed class UpdateAttemptDetailHandler : IRequestHandler<UpdateAttemptDetailCommand, Result>
{
	private readonly IQuizAttemptRepository _quizAttemptRepository;
	private readonly IQuizAttemptSrsService _quizAttemptSrsService;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public UpdateAttemptDetailHandler(
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

	public async Task<Result> Handle(
		UpdateAttemptDetailCommand request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
			return Result.Failure(UserErrors.NotFound);

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var attemptId = QuizAttemptId.From(request.AttemptId);
			var attempt = await _quizAttemptRepository.GetByIdWithItemsAndQuizAsync(attemptId, cancellationToken);
			if (attempt is null || attempt.UserId != userId)
				return Result.Failure(QuizAttemptErrors.NotFound);

			if (attempt.Status == QuizAttemptStatus.Completed)
				return Result.Failure(QuizAttemptErrors.AlreadyCompleted);

			if (attempt.Status == QuizAttemptStatus.Canceled)
				return Result.Failure(QuizAttemptErrors.AlreadyCanceled);

			if (attempt.Status == QuizAttemptStatus.Expired)
				return Result.Failure(QuizAttemptErrors.AttemptExpired);

			if (attempt.Status != QuizAttemptStatus.InProgress)
				return Result.Failure(QuizAttemptErrors.NotInProgress);

			var now = _systemClock.UtcNow;
			if (attempt.IsExpired(now))
			{
				attempt.Expire(now);
				if (attempt.HasAnyAnsweredItems)
				{
					await _quizAttemptSrsService.ApplySrsUpdatesAsync(attempt, now, cancellationToken);
				}
				_quizAttemptRepository.Update(attempt);
				return Result.Failure(QuizAttemptErrors.AttemptExpired);
			}

			var item = attempt.Items.FirstOrDefault(i => i.Id.Value == request.DetailId);
			if (item is null)
				return Result.Failure(QuizAttemptItemErrors.NotFound);

			var question = attempt.Quiz.Questions.FirstOrDefault(q => q.Id == item.QuestionId);
			if (question is null)
				return Result.Failure(QuizErrors.NotFound);

			if (string.IsNullOrWhiteSpace(request.SelectedAnswer) || QuizAnswerHelper.IsSkip(request.SelectedAnswer))
				return Result.Failure(QuizAttemptItemErrors.InvalidSelectedAnswer);

			var typeCode = question.QuestionType?.Code;
			bool isCorrect = CheckIsCorrect(request.SelectedAnswer, question.CorrectAnswer, question.Answers, typeCode);
			int timeSpentMs = Math.Max(0, request.TimeSpentMs);
			string storedAnswer = request.SelectedAnswer.Trim();

			item.UpdateAnswer(storedAnswer, isCorrect, timeSpentMs, now);
			attempt.RecalculateProgress(now);

			_quizAttemptRepository.Update(attempt);
			return Result.Success();
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
