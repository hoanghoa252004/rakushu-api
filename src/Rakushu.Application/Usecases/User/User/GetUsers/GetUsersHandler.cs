using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.User.User.GetUserById;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Identity.User;

namespace Rakushu.Application.Usecases.User.User.GetUsers;

internal sealed class GetUsersHandler : IRequestHandler<GetUsersQuery, Result<PaginatedList<UserListItemDto>>>
{
	// DAOs
	private readonly IUserRepository _userRepository;


	// SERVICES
	private readonly IStorageService _storageService;
	public GetUsersHandler(
		IUserRepository userRepository,
		IStorageService storageService
		)
	{
		_userRepository = userRepository;
		_storageService = storageService;
	}

	public async Task<Result<PaginatedList<UserListItemDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
	{
		UserStatus? parsedStatus = null;
		if (!string.IsNullOrWhiteSpace(request.Status))
		{
			if (!Enum.TryParse<UserStatus>(request.Status, true, out var s))
				return Result.Failure<PaginatedList<UserListItemDto>>(UserErrors.InvalidStatus);
			parsedStatus = s;
		}

		var (pagedUsers, totalCount) = await _userRepository.GetPagedAsync(
			request.RoleId,
			parsedStatus,
			request.SearchTerm,
			request.PageNumber,
			request.PageSize,
			cancellationToken);

		var items = pagedUsers
			.Select(c => new UserListItemDto(
				c.Id.Value,
				c.FullName,
				c.Email,
				c.Status.ToString(),
				c.Profile?.AvatarKey,
				c.CreatedAt,
				c.UpdatedAt,
				new RoleDto(
					c.Role.Id.Value,
					c.Role.Name,
					c.Role.Code
				)
			)).ToList();

		List<UserListItemDto> users = new List<UserListItemDto>();

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

		var result = PaginatedList<UserListItemDto>.Create(
			users,
			totalCount,
			request.PageNumber,
			request.PageSize
			);

		return Result.Success(result);
	}
}
