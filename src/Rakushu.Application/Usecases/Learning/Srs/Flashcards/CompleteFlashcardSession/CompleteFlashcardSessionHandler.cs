using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.FlashcardSession;

namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards.CompleteFlashcardSession;

internal sealed class CompleteFlashcardSessionHandler : IRequestHandler<CompleteFlashcardSessionCommand, Result>
{
	private readonly IFlashcardSessionRepository _flashcardSessionRepository;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public CompleteFlashcardSessionHandler(
		IFlashcardSessionRepository flashcardSessionRepository,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_flashcardSessionRepository = flashcardSessionRepository;
		_currentUserContext = currentUserContext;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(
		CompleteFlashcardSessionCommand request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
			return Result.Failure(UserErrors.NotFound);

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var now = _systemClock.UtcNow;

			var session = await _flashcardSessionRepository.GetByIdWithItemsAsync(
				FlashcardSessionId.From(request.SessionId),
				cancellationToken);

			if (session is null)
				return Result.Failure(FlashcardSessionErrors.NotFound);

			if (session.UserId != userId)
				return Result.Failure(FlashcardSessionErrors.NotFound);

			if (session.IsExpired(now))
			{
				if (!session.HasAnyReviewedItems)
				{
					_flashcardSessionRepository.Delete(session);
				}
				else
				{
					var unrated = session.ExpireAndPrune(now);
					if (unrated.Count > 0)
					{
						_flashcardSessionRepository.RemoveItems(unrated);
					}
					_flashcardSessionRepository.Update(session);
				}
				return Result.Failure(FlashcardSessionErrors.Expired);
			}

			if (!session.HasAnyReviewedItems)
			{
				return Result.Failure(FlashcardSessionErrors.NoItemsReviewed);
			}

			var unratedItems = session.PruneUnratedItems();
			var result = session.Complete(now);
			if (result.IsFailure)
				return result;

			if (unratedItems.Count > 0)
			{
				_flashcardSessionRepository.RemoveItems(unratedItems);
			}

			_flashcardSessionRepository.Update(session);
			return Result.Success();
		}, cancellationToken);
	}
}
