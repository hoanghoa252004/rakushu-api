using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Users.GetUserById;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Plan;
using Rakushu.Domain.Entities.Role;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Application.Usecases.Users.GetUsers;

internal sealed class GetUsersHandler : IRequestHandler<GetUsersQuery, Result<PaginatedList<UserDto>>>
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

	public async Task<Result<PaginatedList<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(request.Status) == false && !Enum.TryParse<UserStatus>(request.Status, true, out var status))
		{
			return Result.Failure<PaginatedList<UserDto>>(UserError.InvalidStatus);
		}

		var (items, totalCount) = await _userQuery.GetUsersAsync(request, cancellationToken);

		var users = new List<UserDto>();

		foreach (var user in items)
		{

			if(user.AvatarUrl == null)
			{
				users.Add(user);
			}
			else
			{
				var avatarUrl = await _storageService.CreatePresignedReadUrlAsync(user.AvatarUrl, cancellationToken);

				users.Add(new UserDto(
					user.Id,
					user.Email,
					user.FullName,
					user.Role,
					user.Status,
					user.CreatedAt,
					user.UpdatedAt,
					avatarUrl,
					user.NativeLanguage
				));
			}
		}

		var removedAdminLists = users.Where(p => p.Role != DefaultSystemRoles.SystemAdministrator.ToString()).ToList();

		var result = PaginatedList<UserDto>.Create(
			removedAdminLists,
			totalCount,
			request.PageNumber,
			request.PageSize
			);

		return Result.Success(result);
	}
}
