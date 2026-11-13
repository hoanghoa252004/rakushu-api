using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.SavedItem;

namespace Rakushu.Application.Usecases.Learning.Srs.SavedItem.ToggleSavedItemActive;

internal sealed class ToggleSavedItemActiveHandler : IRequestHandler<ToggleSavedItemActiveCommand, Result>
{
	private readonly ISavedItemRepository _savedItemRepository;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly IUnitOfWork _unitOfWork;

	public ToggleSavedItemActiveHandler(
		ISavedItemRepository savedItemRepository,
		ICurrentUserContext currentUserContext,
		IUnitOfWork unitOfWork)
	{
		_savedItemRepository = savedItemRepository;
		_currentUserContext = currentUserContext;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(
		ToggleSavedItemActiveCommand request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
			return Result.Failure(UserErrors.NotFound);

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var itemId = SavedItemId.From(request.Id);
			var item = await _savedItemRepository.GetByIdAsync(itemId, cancellationToken);
			if (item is null || item.UserId != userId)
				return Result.Failure(SavedItemErrors.NotFound);

			if (request.IsActive && item.IsRevoked)
			{
				return Result.Failure(SavedItemErrors.CannotActivateRevokedMeaning);
			}

			item.ToggleActive(request.IsActive);
			_savedItemRepository.Update(item);

			return Result.Success();
		}, cancellationToken);
	}
}
