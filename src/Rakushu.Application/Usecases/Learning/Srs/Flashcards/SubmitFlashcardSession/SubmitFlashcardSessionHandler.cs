using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.FlashcardSession;

namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards.SubmitFlashcardSession;

internal sealed class SubmitFlashcardSessionHandler : IRequestHandler<SubmitFlashcardSessionCommand, Result>
{
	private readonly IFlashcardSessionRepository _flashcardSessionRepository;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;
	private readonly IFlashcardSrsService _flashcardSrsService;
	private readonly IUnitOfWork _unitOfWork;

	public SubmitFlashcardSessionHandler(
		IFlashcardSessionRepository flashcardSessionRepository,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock,
		IFlashcardSrsService flashcardSrsService,
		IUnitOfWork unitOfWork)
	{
		_flashcardSessionRepository = flashcardSessionRepository;
		_currentUserContext = currentUserContext;
		_systemClock = systemClock;
		_flashcardSrsService = flashcardSrsService;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(
		SubmitFlashcardSessionCommand request,
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
				session.Expire(now);
				if (session.HasAnyReviewedItems)
				{
					await _flashcardSrsService.ApplySrsUpdatesAsync(session, now, cancellationToken);
				}
				_flashcardSessionRepository.Update(session);
				return Result.Failure(FlashcardSessionErrors.Expired);
			}

			var result = session.Complete(now);
			if (result.IsFailure)
				return result;

			// Apply SM-2 updates to all reviewed items upon completion
			await _flashcardSrsService.ApplySrsUpdatesAsync(session, now, cancellationToken);

			_flashcardSessionRepository.Update(session);
			return Result.Success();
		}, cancellationToken);
	}
}
