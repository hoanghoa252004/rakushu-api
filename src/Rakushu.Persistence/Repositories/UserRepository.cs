using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities;
using Rakushu.Domain.Repositories;
using Rakushu.Persistence.DbContext;

namespace Rakushu.Persistence.Repositories;

public sealed class UserRepository : Repository<User, Guid>, IUserRepository
{
	public UserRepository(RakushuDbContext context) : base(context)
	{
	}

	public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
	{
		return await _dbSet
			.Include(u => u.Role)
			.Include(u => u.Profile)
			.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower(), cancellationToken);
	}

	public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
	{
		return await _dbSet
			.Include(u => u.Role)
			.Include(u => u.Profile)
			.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower(), cancellationToken);
	}

	public async Task<User?> GetByIdWithProfileAndRoleAsync(Guid id, CancellationToken cancellationToken = default)
	{
		return await _dbSet
			.Include(u => u.Role)
			.Include(u => u.Profile)
			.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
	}

	public async Task<bool> IsEmailUniqueAsync(string email, Guid? excludeUserId = null, CancellationToken cancellationToken = default)
	{
		var query = _dbSet.Where(u => u.Email.ToLower() == email.ToLower());
		if (excludeUserId.HasValue)
		{
			query = query.Where(u => u.Id != excludeUserId.Value);
		}
		return !await query.AnyAsync(cancellationToken);
	}

	public async Task<bool> IsUsernameUniqueAsync(string username, Guid? excludeUserId = null, CancellationToken cancellationToken = default)
	{
		var query = _dbSet.Where(u => u.Username.ToLower() == username.ToLower());
		if (excludeUserId.HasValue)
		{
			query = query.Where(u => u.Id != excludeUserId.Value);
		}
		return !await query.AnyAsync(cancellationToken);
	}

	public async Task<(IEnumerable<User> Items, int TotalCount)> GetPagedAsync(
		int pageNumber,
		int pageSize,
		string? searchTerm = null,
		Guid? roleId = null,
		string? status = null,
		CancellationToken cancellationToken = default)
	{
		var query = _dbSet
			.Include(u => u.Role)
			.Include(u => u.Profile)
			.AsQueryable();

		if (!string.IsNullOrWhiteSpace(searchTerm))
		{
			var search = searchTerm.Trim().ToLower();
			query = query.Where(u =>
				u.Username.ToLower().Contains(search) ||
				u.Email.ToLower().Contains(search) ||
				(u.Profile != null && u.Profile.DisplayName.ToLower().Contains(search)));
		}

		if (roleId.HasValue && roleId.Value != Guid.Empty)
		{
			query = query.Where(u => u.RoleId == roleId.Value);
		}

		if (!string.IsNullOrWhiteSpace(status))
		{
			query = query.Where(u => u.Status.ToLower() == status.Trim().ToLower());
		}

		var totalCount = await query.CountAsync(cancellationToken);

		var items = await query
			.OrderByDescending(u => u.CreatedAt)
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync(cancellationToken);

		return (items, totalCount);
	}
}
