using MediatR;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Admin.Users.GetUserById;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Application.Usecases.Admin.Users.GetUsers;

internal sealed class GetUsersHandler : IRequestHandler<GetUsersQuery, Result<PaginatedList<UserDto>>>
{
	// DAOs
	private readonly IUserQuery _userQuery;

	public GetUsersHandler(IUserQuery userQuery)
	{
		_userQuery = userQuery;
	}

	public async Task<Result<PaginatedList<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
	{
		var (items, totalCount) = await _userQuery.GetUsersAsync(request, cancellationToken);

		var result = PaginatedList<UserDto>.Create(
			items,
			totalCount,
			request.PageNumber,
			request.PageSize
			);

		return Result.Success(result);
	}
}
