using Microsoft.EntityFrameworkCore;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Application.Usecases.Admin.Users.GetUserById;
using Rakushu.Application.Usecases.Admin.Users.GetUsers;
using Rakushu.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Persistence.Queries;

internal class UserQuery : IUserQuery
{
	private readonly RakushuDbContext _dbContext;

	public UserQuery(RakushuDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<UserDto?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default)
	{
		return await _dbContext.Users
			.AsNoTracking()
			.Select(u => new UserDto(
				u.Id.Value,
				u.Email.Value,
				u.Profile.FullName,
				u.Role.Title,
				u.Status.ToString(),
				u.CreatedAt,
				u.UpdatedAt,
				u.Profile.AvatarKey,
				u.Profile.NativeLanguage
				))
			.SingleOrDefaultAsync(u => u.Id == id.Value, cancellationToken);
	}

	public async Task<(IReadOnlyList<UserDto> Items, int TotalCount)> GetUsersAsync(GetUsersQuery query, CancellationToken cancellationToken = default)
	{
		IQueryable<User> users = _dbContext.Users.AsNoTracking();

		// Search
		if (!string.IsNullOrWhiteSpace(query.SearchTerm))
		{
			var searchTerm = query.SearchTerm.Trim();

			users = users.Where(u =>
				u.Email.Value.Contains(searchTerm) ||
				u.Profile.FullName.Contains(searchTerm));
		}

		// Filter by role
		if (query.RoleId != null)
		{
			users = users.Where(u => u.RoleId.Value == query.RoleId);
		}

		// Filter by status
		if (query.Status != null)
		{
			users = users.Where(u => u.Status == query.Status);
		}

		// Count BEFORE pagination
		var totalCount = await users
			.CountAsync(cancellationToken);

		// Pagination + Projection
		var items = await users
			.OrderBy(u => u.Id)
			.Skip((query.PageNumber - 1) * query.PageSize)
			.Take(query.PageSize)
			.Select(u => new UserDto(
				u.Id.Value,
				u.Email.Value,
				u.Profile.FullName,
				u.Role.Title,
				u.Status.ToString(),
				u.CreatedAt,
				u.UpdatedAt,
				u.Profile.AvatarKey,
				u.Profile.NativeLanguage))
			.ToListAsync(cancellationToken);

		return (items, totalCount);
	}
}

