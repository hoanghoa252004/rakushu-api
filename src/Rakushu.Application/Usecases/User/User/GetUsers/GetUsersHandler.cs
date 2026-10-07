using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.User.User.GetUserById;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Role;
using Rakushu.Domain.Entities.User;

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
		if (string.IsNullOrWhiteSpace(request.Status) == false && !Enum.TryParse<UserStatus>(request.Status, true, out var status))
		{
			return Result.Failure<PaginatedList<UserListItemDto>>(UserErrors.InvalidStatus);
		}

		var list = (await _userRepository.GetAllAsync(cancellationToken))
			.Where(u => u.Role.Code != RoleCodes.SystemAdministrator);

		if (request.RoleId.HasValue)
		{
			list = list.Where(p => p.RoleId.Value == request.RoleId);
		}

		if (!string.IsNullOrWhiteSpace(request.Status))
		{
			list = list.Where(p => p.Status.ToString().ToLower() == request.Status.ToLower());
		}

		if (!string.IsNullOrWhiteSpace(request.SearchTerm))
		{
			var key = request.SearchTerm.Trim();
			list = list.Where(p => p.FullName.Contains(key, StringComparison.OrdinalIgnoreCase)
									|| p.Email.Contains(key, StringComparison.OrdinalIgnoreCase)
									|| p.Role.Name.Contains(key, StringComparison.OrdinalIgnoreCase));
		}

		// Calculate total count before paging
		var totalCount = list.Count();

		// Apply paging
		var items = list
			.OrderBy(c => c.UpdatedAt)
			.Skip((request.PageNumber - 1) * request.PageSize)
			.Take(request.PageSize)
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
