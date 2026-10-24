using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Usecases.Learning.Srs.Common;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs;

namespace Rakushu.Application.Usecases.Learning.Srs.ResumeSrsItem;

internal sealed class ResumeSrsItemHandler
	: IRequestHandler<ResumeSrsItemCommand, Result<SrsItemDto>>
{
	private readonly ICurrentUserContext _currentUser;
	private readonly ISrsItemRepository _repository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ISystemClock _clock;

	public ResumeSrsItemHandler(
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

	public async Task<Result<SrsItemDto>> Handle(
		ResumeSrsItemCommand request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUser.UserId;
		if (userId is null)
		{
			return Result.Failure<SrsItemDto>(UserErrors.UnauthorizedResourceAccess);
		}

		var item = await _repository.GetByIdForUserAsync(
			SrsItemId.From(request.ItemId),
			userId,
			cancellationToken);

		if (item is null)
		{
			return Result.Failure<SrsItemDto>(SrsItemErrors.NotFound);
		}

		var now = _clock.UtcNow;
		var resumeResult = item.Resume(now);
		if (resumeResult.IsFailure)
		{
			return Result.Failure<SrsItemDto>(resumeResult.Error);
		}

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			_repository.Update(item);
			return Result.Success(SrsItemDto.FromEntity(item));
		}, cancellationToken);
	}
}
