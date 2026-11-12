using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.FlashcardSession;

namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards.CancelFlashcardSession;

internal sealed class CancelFlashcardSessionHandler : IRequestHandler<CancelFlashcardSessionCommand, Result>
{
	private readonly IFlashcardSessionRepository _flashcardSessionRepository;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public CancelFlashcardSessionHandler(
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
		CancelFlashcardSessionCommand request,
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

			var result = session.Cancel(now);
			if (result.IsFailure)
				return result;

			_flashcardSessionRepository.Update(session);
			return Result.Success();
		}, cancellationToken);
	}
}
