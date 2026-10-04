using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.User.User.GetUserById;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Role;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Application.Usecases.User.User.GetUsers;

internal sealed class GetUsersHandler : IRequestHandler<GetUsersQuery, Result<PaginatedList<UserListItemDto>>>
{
	// DAOs
	private readonly IUserQuery _userQuery;


	// SERVICES
	private readonly IStorageService _storageService;
	public GetUsersHandler(
		IUserQuery userQuery,
		IStorageService storageService
		)
	{
		_userQuery = userQuery;
		_storageService = storageService;
	}

	public async Task<Result<PaginatedList<UserListItemDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(request.Status) == false && !Enum.TryParse<UserStatus>(request.Status, true, out var status))
		{
			return Result.Failure<PaginatedList<UserListItemDto>>(UserErrors.InvalidStatus);
		}

		var (items, totalCount) = await _userQuery.GetUsersAsync(request, cancellationToken);

		var users = new List<UserListItemDto>();

		foreach (var user in items)
		{
			// Process avatar URL if profile exists
			if(user.Avatar != null)
			{
				var avatarUrl = await _storageService.CreatePresignedReadUrlAsync(user.Avatar, cancellationToken);
				var updatedUser = user with { Avatar = avatarUrl };
				users.Add(updatedUser);
			}
			else
			{
				users.Add(user);
			}
		}

		var removedAdminLists = users.Where(p => p.Role.Code != RoleCodes.SystemAdministrator).ToList();

		var result = PaginatedList<UserListItemDto>.Create(
			removedAdminLists,
			totalCount,
			request.PageNumber,
			request.PageSize
			);

		return Result.Success(result);
	}
}
