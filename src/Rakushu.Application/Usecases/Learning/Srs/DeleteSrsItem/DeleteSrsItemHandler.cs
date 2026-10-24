using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs;

namespace Rakushu.Application.Usecases.Learning.Srs.DeleteSrsItem;

internal sealed class DeleteSrsItemHandler
	: IRequestHandler<DeleteSrsItemCommand, Result>
{
	private readonly ICurrentUserContext _currentUser;
	private readonly ISrsItemRepository _repository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ISystemClock _clock;

	public DeleteSrsItemHandler(
		ICurrentUserContext currentUser,
		ISrsItemRepository repository,
		IUnitOfWork unitOfWork,
		ISystemClock clock)
	{
		_currentUser = currentUser;
		_repository = repository;
		_unitOfWork = unitOfWork;
		_clock = clock;
	}

	public async Task<Result> Handle(
		DeleteSrsItemCommand request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUser.UserId;
		if (userId is null)
		{
			return Result.Failure(UserErrors.UnauthorizedResourceAccess);
		}

		var item = await _repository.GetByIdForUserAsync(
			SrsItemId.From(request.ItemId),
			userId,
			cancellationToken);

		if (item is null)
		{
			return Result.Failure(SrsItemErrors.NotFound);
		}

		var now = _clock.UtcNow;
		var hasQuizHistory = await _repository.HasQuizHistoryAsync(item.Id, cancellationToken);

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			if (hasQuizHistory)
			{
				// Item has foreign keys in quiz history: safely soft-delete via Ignored
				var ignoreResult = item.Ignore(now);
				if (ignoreResult.IsFailure)
				{
					return ignoreResult;
				}

				_repository.Update(item);
			}
			else
			{
				// No historic quiz references: safe to permanently hard-delete
				_repository.Delete(item);
			}

			return Result.Success();
		}, cancellationToken);
	}
}
