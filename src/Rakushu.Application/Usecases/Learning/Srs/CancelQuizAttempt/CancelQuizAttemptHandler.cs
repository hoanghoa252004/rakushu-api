using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.QuizAttempt;

namespace Rakushu.Application.Usecases.Learning.Srs.CancelQuizAttempt;

internal sealed class CancelQuizAttemptHandler : IRequestHandler<CancelQuizAttemptCommand, Result>
{
	private readonly IQuizAttemptRepository _quizAttemptRepository;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public CancelQuizAttemptHandler(
		IQuizAttemptRepository quizAttemptRepository,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_quizAttemptRepository = quizAttemptRepository;
		_currentUserContext = currentUserContext;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(
		CancelQuizAttemptCommand request,
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

			if (attempt.Status != QuizAttemptStatus.InProgress)
				return Result.Failure(QuizAttemptErrors.NotInProgress);

			var now = _systemClock.UtcNow;
			attempt.Abandon(now);

			return Result.Success();
		}, cancellationToken);
	}
}
