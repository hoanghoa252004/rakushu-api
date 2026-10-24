using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Usecases.Learning.Srs.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs;

namespace Rakushu.Application.Usecases.Learning.Srs.GetItems;

internal sealed class GetItemsHandler
	: IRequestHandler<GetItemsQuery, Result<IReadOnlyList<SrsItemDto>>>
{
	// CONTEXT
	private readonly ICurrentUserContext _currentUser;

	// REPOSITORIES
	private readonly ISrsItemRepository _repository;

	public GetItemsHandler(
		ICurrentUserContext currentUser,
		ISrsItemRepository repository)
	{
		_currentUser = currentUser;
		_repository = repository;
	}

	public async Task<Result<IReadOnlyList<SrsItemDto>>> Handle(
		GetItemsQuery request,
		CancellationToken cancellationToken)
	{
		// 1. Validate User Context
		var userId = _currentUser.UserId;
		if (userId is null)
		{
			return Result.Failure<IReadOnlyList<SrsItemDto>>(
				UserErrors.UnauthorizedResourceAccess);
		}

		// 2. Fetch Active Items for User
		var items = await _repository.GetItemsAsync(userId, cancellationToken);

		// 3. Return DTOs
		return Result.Success<IReadOnlyList<SrsItemDto>>(
			items.Select(SrsItemDto.FromEntity).ToList());
	}
}
