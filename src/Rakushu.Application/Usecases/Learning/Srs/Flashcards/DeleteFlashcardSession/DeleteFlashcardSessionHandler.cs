using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.FlashcardSession;

namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards.DeleteFlashcardSession;

internal sealed class DeleteFlashcardSessionHandler : IRequestHandler<DeleteFlashcardSessionCommand, Result>
{
	private readonly IFlashcardSessionRepository _flashcardSessionRepository;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly IUnitOfWork _unitOfWork;

	public DeleteFlashcardSessionHandler(
		IFlashcardSessionRepository flashcardSessionRepository,
		ICurrentUserContext currentUserContext,
		IUnitOfWork unitOfWork)
	{
		_flashcardSessionRepository = flashcardSessionRepository;
		_currentUserContext = currentUserContext;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(
		DeleteFlashcardSessionCommand request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
			return Result.Failure(UserErrors.NotFound);

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var session = await _flashcardSessionRepository.GetByIdWithItemsAsync(
				FlashcardSessionId.From(request.SessionId),
				cancellationToken);

			if (session is null)
				return Result.Failure(FlashcardSessionErrors.NotFound);

			if (session.UserId != userId)
				return Result.Failure(FlashcardSessionErrors.NotFound);

			_flashcardSessionRepository.Delete(session);
			return Result.Success();
		}, cancellationToken);
	}
}
