using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.FlashcardSession;
using Rakushu.Domain.Entities.Srs.FlashcardSession.FlashcardSessionCard;
using Rakushu.Domain.Entities.Srs.SavedItem;

namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards.CreateFlashcardSession;

internal sealed class CreateFlashcardSessionHandler : IRequestHandler<CreateFlashcardSessionCommand, Result<Guid>>
{
	private readonly IFlashcardSessionRepository _flashcardSessionRepository;
	private readonly ISavedItemRepository _savedItemRepository;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public CreateFlashcardSessionHandler(
		IFlashcardSessionRepository flashcardSessionRepository,
		ISavedItemRepository savedItemRepository,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_flashcardSessionRepository = flashcardSessionRepository;
		_savedItemRepository = savedItemRepository;
		_currentUserContext = currentUserContext;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<Guid>> Handle(
		CreateFlashcardSessionCommand request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
			return Result.Failure<Guid>(UserErrors.NotFound);

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var now = _systemClock.UtcNow;

			// Check for active session
			var activeSession = await _flashcardSessionRepository.GetActiveSessionByUserAsync(userId, cancellationToken);
			if (activeSession is not null)
			{
				if (activeSession.IsExpired(now))
				{
					if (!activeSession.HasAnyReviewedItems)
					{
						_flashcardSessionRepository.Delete(activeSession);
					}
					else
					{
						activeSession.Expire(now);
						_flashcardSessionRepository.Update(activeSession);
					}
				}
				else
				{
					return Result.Failure<Guid>(FlashcardSessionErrors.ActiveSessionInProgress);
				}
			}

			// Validate input items
			var distinctItemIds = request.ItemIds.Distinct().Select(SavedItemId.From).ToList();
			if (distinctItemIds.Count == 0)
			{
				return Result.Failure<Guid>(FlashcardSessionErrors.EmptyItems);
			}

			// Retrieve saved items with schedules
			var savedItems = await _savedItemRepository.GetByIdsWithScheduleAsync(distinctItemIds, userId, cancellationToken);

			// Filter eligible items (active)
			var eligibleItems = savedItems
				.Where(item => item.IsActive)
				.ToList();

			if (eligibleItems.Count == 0)
			{
				return Result.Failure<Guid>(FlashcardSessionErrors.EmptyItems);
			}

			// Create Flashcard Session Aggregate
			var sessionResult = FlashcardSession.Create(userId, now);
			if (sessionResult.IsFailure)
				return Result.Failure<Guid>(sessionResult.Error);

			var session = sessionResult.Value;

			// Add cards (the cards to study in this session)
			foreach (var savedItem in eligibleItems)
			{
				session.AddCard(FlashcardSessionCard.Create(session.Id, savedItem.Id));
			}

			_flashcardSessionRepository.Add(session);

			return Result.Success(session.Id.Value);
		}, cancellationToken);
	}
}
