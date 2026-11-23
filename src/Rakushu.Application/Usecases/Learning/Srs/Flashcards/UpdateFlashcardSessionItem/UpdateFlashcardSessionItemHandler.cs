using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Usecases.Learning.Srs.Flashcards.SubmitFlashcardSession;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.FlashcardSession;
using Rakushu.Domain.Entities.Srs.FlashcardSession.FlashcardSessionItem;

namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards.UpdateFlashcardSessionItem;

internal sealed class UpdateFlashcardSessionItemHandler : IRequestHandler<UpdateFlashcardSessionItemCommand, Result>
{
	private readonly IFlashcardSessionRepository _flashcardSessionRepository;
	private readonly IFlashcardSrsService _flashcardSrsService;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public UpdateFlashcardSessionItemHandler(
		IFlashcardSessionRepository flashcardSessionRepository,
		IFlashcardSrsService flashcardSrsService,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_flashcardSessionRepository = flashcardSessionRepository;
		_flashcardSrsService = flashcardSrsService;
		_currentUserContext = currentUserContext;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(
		UpdateFlashcardSessionItemCommand request,
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

			if (session.Status != FlashcardSessionStatus.InProgress)
				return Result.Failure(FlashcardSessionErrors.NotInProgress);

			if (session.IsExpired(now))
			{
				session.Expire(now);
				if (session.HasAnyReviewedItems)
				{
					await _flashcardSrsService.ApplySrsUpdatesAsync(session, now, cancellationToken);
				}
				_flashcardSessionRepository.Update(session);
				return Result.Failure(FlashcardSessionErrors.Expired);
			}

			var sessionItem = session.Items.FirstOrDefault(i => i.Id.Value == request.ItemId);
			if (sessionItem is null)
				return Result.Failure(FlashcardSessionErrors.ItemNotFound);

			var rating = ParseRating(request.Rating);

			sessionItem.UpdateRating(rating, now);

			_flashcardSessionRepository.Update(session);

			return Result.Success();
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
