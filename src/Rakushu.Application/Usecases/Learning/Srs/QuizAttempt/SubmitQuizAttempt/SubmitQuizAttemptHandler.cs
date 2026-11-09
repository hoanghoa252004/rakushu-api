using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.QuizAttempt;

namespace Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.SubmitQuizAttempt;

internal sealed class SubmitQuizAttemptHandler : IRequestHandler<SubmitQuizAttemptCommand, Result>
{
	private readonly IQuizAttemptRepository _quizAttemptRepository;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;
	private readonly IQuizAttemptSrsService _quizAttemptSrsService;
	private readonly IUnitOfWork _unitOfWork;

	public SubmitQuizAttemptHandler(
		IQuizAttemptRepository quizAttemptRepository,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock,
		IQuizAttemptSrsService quizAttemptSrsService,
		IUnitOfWork unitOfWork)
	{
		_quizAttemptRepository = quizAttemptRepository;
		_currentUserContext = currentUserContext;
		_systemClock = systemClock;
		_quizAttemptSrsService = quizAttemptSrsService;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(
		SubmitQuizAttemptCommand request,
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

			if (!attempt.HasAnyAnsweredItems)
			{
				return Result.Failure(QuizAttemptErrors.NoItemsAnswered);
			}

			var unansweredItems = attempt.PruneUnansweredItems();
			if (unansweredItems.Count > 0)
			{
				_quizAttemptRepository.RemoveItems(unansweredItems);
			}

			await _quizAttemptSrsService.ApplySrsUpdatesAsync(attempt, now, cancellationToken);

			var completeResult = attempt.Complete(now);
			if (completeResult.IsFailure)
				return completeResult;

			_quizAttemptRepository.Update(attempt);


			return Result.Success();
		}, cancellationToken);
	}
}
