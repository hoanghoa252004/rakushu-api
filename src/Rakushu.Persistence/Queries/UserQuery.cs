using Microsoft.EntityFrameworkCore;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Application.Usecases.Admin.Users.GetUserById;
using Rakushu.Application.Usecases.Admin.Users.GetUsers;
using Rakushu.Domain.Entities.Role;
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
			.Where(u => u.Id == id)
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
			.SingleOrDefaultAsync(cancellationToken);
	}

	public async Task<(IReadOnlyList<UserDto> Items, int TotalCount)> GetUsersAsync(GetUsersQuery query, CancellationToken cancellationToken = default)
	{
		IQueryable<User> users = _dbContext.Users.AsNoTracking();

		// Search
		if (!string.IsNullOrWhiteSpace(query.SearchTerm))
		{
			var searchTerm = query.SearchTerm.Trim();

			//var pattern = $"%{searchTerm}%";

			//users = _dbContext.Users.FromSqlInterpolated($"""
			//	SELECT *
			//	FROM users
			//	WHERE email ILIKE {pattern}
			//	   OR profile_full_name ILIKE {pattern}
			//	""").AsNoTracking();

			users = users.Where(
				//u =>
				//u.Email.Value.Contains(searchTerm) ||
				//u.Profile.FullName.Contains(searchTerm)
				u => EF.Functions.ILike(u.Profile.FullName, $"%{searchTerm}%")
				//|| EF.Functions.ILike(EF.Property<string>(u, nameof(User.Email)), $"%{searchTerm}%")
				);
		}

		// Filter by role
		if (query.RoleId != null)
		{
			var roleId = RoleId.From(query.RoleId.Value);

			users = users.Where(u => u.RoleId == roleId);
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

