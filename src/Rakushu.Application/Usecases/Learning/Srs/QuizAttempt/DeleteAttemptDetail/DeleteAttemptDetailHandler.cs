using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.SubmitQuizAttempt;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.QuizAttempt;
using Rakushu.Domain.Entities.Srs.QuizAttempt.QuizAttemptItem;

namespace Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.DeleteAttemptDetail;

internal sealed class DeleteAttemptDetailHandler : IRequestHandler<DeleteAttemptDetailCommand, Result>
{
	private readonly IQuizAttemptRepository _quizAttemptRepository;
	private readonly IQuizAttemptSrsService _quizAttemptSrsService;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public DeleteAttemptDetailHandler(
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
		DeleteAttemptDetailCommand request,
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

			attempt.RemoveItem(item.Id, now);
			_quizAttemptRepository.RemoveItems([item]);
			_quizAttemptRepository.Update(attempt);

			return Result.Success();
		}, cancellationToken);
	}
}
