using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.FlashcardSession;
using Rakushu.Domain.Entities.Srs.FlashcardSession.FlashcardSessionItem;
using Rakushu.Domain.Entities.Srs.SavedItem;

namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards.CreateFlashcardSessionItem;

internal sealed class CreateFlashcardSessionItemHandler : IRequestHandler<CreateFlashcardSessionItemCommand, Result<Guid>>
{
	private readonly IFlashcardSessionRepository _flashcardSessionRepository;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public CreateFlashcardSessionItemHandler(
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

	public async Task<Result<Guid>> Handle(
		CreateFlashcardSessionItemCommand request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
			return Result.Failure<Guid>(UserErrors.NotFound);

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var now = _systemClock.UtcNow;

			var session = await _flashcardSessionRepository.GetByIdWithItemsAsync(
				FlashcardSessionId.From(request.SessionId),
				cancellationToken);

			if (session is null)
				return Result.Failure<Guid>(FlashcardSessionErrors.NotFound);

			if (session.UserId != userId)
				return Result.Failure<Guid>(FlashcardSessionErrors.NotFound);

			if (session.Status != FlashcardSessionStatus.InProgress)
				return Result.Failure<Guid>(FlashcardSessionErrors.NotInProgress);

			if (session.IsExpired(now))
			{
				if (!session.HasAnyReviewedItems)
				{
					_flashcardSessionRepository.Delete(session);
				}
				else
				{
					session.Expire(now);
					_flashcardSessionRepository.Update(session);
				}
				return Result.Failure<Guid>(FlashcardSessionErrors.Expired);
			}

			var savedItemId = SavedItemId.From(request.SavedItemId);

			// Validate target item exists in this session
			if (!session.Targets.Any(t => t.SavedItemId == savedItemId))
				return Result.Failure<Guid>(FlashcardSessionErrors.TargetNotFound);

			// Check if already reviewed in this session
			if (session.Items.Any(i => i.SavedItemId == savedItemId))
				return Result.Failure<Guid>(FlashcardSessionErrors.ItemAlreadyReviewed);

			var rating = ParseRating(request.Rating);

			var sessionItemResult = FlashcardSessionItem.Create(session.Id, savedItemId, rating, now);
			if (sessionItemResult.IsFailure)
				return Result.Failure<Guid>(sessionItemResult.Error);

			var sessionItem = sessionItemResult.Value;
			session.AddItem(sessionItem);
			session.RefreshIdleTimeout(now);

			_flashcardSessionRepository.Update(session);

			return Result.Success(sessionItem.Id.Value);
		}, cancellationToken);
	}

	private static FlashcardRating ParseRating(string ratingStr)
	{
		if (ratingStr.Equals("Forgot", StringComparison.OrdinalIgnoreCase))
			return FlashcardRating.Forgot;

		if (ratingStr.Equals("Hard", StringComparison.OrdinalIgnoreCase))
			return FlashcardRating.Hard;

		return FlashcardRating.Easy;
	}
}
