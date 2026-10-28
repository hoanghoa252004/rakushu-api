using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.SavedItem;

namespace Rakushu.Application.Usecases.Learning.Srs.DeleteSrsItem;

internal sealed class DeleteSrsItemHandler : IRequestHandler<DeleteSrsItemCommand, Result>
{
	private readonly ISavedItemRepository _savedItemRepository;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public DeleteSrsItemHandler(
		ISavedItemRepository savedItemRepository,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_savedItemRepository = savedItemRepository;
		_currentUserContext = currentUserContext;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(
		DeleteSrsItemCommand request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
			return Result.Failure(UserErrors.NotFound);

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var itemId = SavedItemId.From(request.Id);
			var item = await _savedItemRepository.GetByIdWithScheduleAsync(itemId, cancellationToken);
			if (item is null || item.UserId != userId)
				return Result.Failure(SavedItemErrors.NotFound);

			var now = _systemClock.UtcNow;

			// Per diagram: Learner deletes word transitions SrsSchedule to Ignored
			if (item.Schedule is not null)
			{
				item.Schedule.MarkAsIgnored(now);
			}
			else
			{
				_savedItemRepository.Delete(item);
			}

			return Result.Success();
		}, cancellationToken);
	}
}
