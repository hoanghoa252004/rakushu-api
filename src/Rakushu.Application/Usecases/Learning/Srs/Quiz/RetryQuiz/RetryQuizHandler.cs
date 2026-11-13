using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.SubmitQuizAttempt;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.Quiz;
using Rakushu.Domain.Entities.Srs.QuizAttempt;

namespace Rakushu.Application.Usecases.Learning.Srs.Quiz.RetryQuiz;

internal sealed class RetryQuizHandler : IRequestHandler<RetryQuizCommand, Result<Guid>>
{
	private readonly IQuizRepository _quizRepository;
	private readonly IQuizAttemptRepository _quizAttemptRepository;
	private readonly IQuizAttemptSrsService _quizAttemptSrsService;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public RetryQuizHandler(
		IQuizRepository quizRepository,
		IQuizAttemptRepository quizAttemptRepository,
		IQuizAttemptSrsService quizAttemptSrsService,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_quizRepository = quizRepository;
		_quizAttemptRepository = quizAttemptRepository;
		_quizAttemptSrsService = quizAttemptSrsService;
		_currentUserContext = currentUserContext;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<Guid>> Handle(
		RetryQuizCommand request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
			return Result.Failure<Guid>(UserErrors.NotFound);

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var now = _systemClock.UtcNow;

			var quizId = QuizId.From(request.QuizId);
			var quiz = await _quizRepository.GetByIdWithQuestionsAsync(quizId, cancellationToken);
			if (quiz is null || quiz.UserId != userId)
				return Result.Failure<Guid>(QuizErrors.NotFound);

			var activeAttempt = await _quizAttemptRepository.GetActiveAttemptForUserAsync(userId, cancellationToken);
			if (activeAttempt is not null)
			{
				if (activeAttempt.IsExpired(now))
				{
					activeAttempt.Expire(now);
					if (activeAttempt.HasAnyAnsweredItems)
					{
						await _quizAttemptSrsService.ApplySrsUpdatesAsync(activeAttempt, now, cancellationToken);
					}
					_quizAttemptRepository.Update(activeAttempt);
				}
				else
				{
					return Result.Failure<Guid>(QuizAttemptErrors.ActiveAttemptInProgress);
				}
			}

			var attemptResult = Domain.Entities.Srs.QuizAttempt.QuizAttempt.Create(
				userId,
				quiz.Id,
				now,
				quiz.TotalQuestions,
				QuizAttemptMode.PracticeAll);

			if (attemptResult.IsFailure)
				return Result.Failure<Guid>(attemptResult.Error);

			var attempt = attemptResult.Value;
			_quizAttemptRepository.Add(attempt);

			return Result.Success(attempt.Id.Value);
		}, cancellationToken);
	}
}
