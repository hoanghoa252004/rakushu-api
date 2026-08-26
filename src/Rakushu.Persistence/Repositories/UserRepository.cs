using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.User;
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
		return await _context.Users
			.Include(u => u.Role)
			.Include(u => u.Profile)
			.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower(), cancellationToken);
	}

	public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
	{
		return await _context.Users
			.Include(u => u.Role)
			.Include(u => u.Profile)
			.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower(), cancellationToken);
	}

	public async Task<User?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
	{
		return await _context.Users
			.Include(u => u.Role)
			.Include(u => u.Profile)
			.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
	}

	public async Task<Profile?> GetProfileByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
	{
		return await _context.Profiles
			.FirstOrDefaultAsync(p => p.Id == userId, cancellationToken);
	}

	public async Task<bool> IsEmailUniqueAsync(
		string email,
		Guid? excludeUserId = null,
		CancellationToken cancellationToken = default)
	{
		return !await _context.Users
			.AnyAsync(u => u.Email.ToLower() == email.ToLower() && (!excludeUserId.HasValue || u.Id != excludeUserId.Value), cancellationToken);
	}

	public async Task<bool> IsUsernameUniqueAsync(
		string username,
		Guid? excludeUserId = null,
		CancellationToken cancellationToken = default)
	{
		return !await _context.Users
			.AnyAsync(u => u.Username.ToLower() == username.ToLower() && (!excludeUserId.HasValue || u.Id != excludeUserId.Value), cancellationToken);
	}

	public async Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
	{
		return await _context.RefreshTokens
			.Include(t => t.User)
			.FirstOrDefaultAsync(t => t.Token == token, cancellationToken);
	}

	public async Task RevokeUserRefreshTokensAsync(Guid userId, CancellationToken cancellationToken = default)
	{
		var tokens = await _context.RefreshTokens
			.Where(t => t.UserId == userId && !t.IsRevoked)
			.ToListAsync(cancellationToken);

		foreach (var token in tokens)
		{
			token.Revoke();
		}
	}

	public void AddRefreshToken(RefreshToken token)
	{
		_context.RefreshTokens.Add(token);
	}

	public async Task<(IReadOnlyList<User> Items, int TotalCount)> GetPagedAsync(
		int pageNumber,
		int pageSize,
		string? searchTerm = null,
		Guid? roleId = null,
		string? status = null,
		CancellationToken cancellationToken = default)
	{
		var query = _context.Users
			.Include(u => u.Role)
			.Include(u => u.Profile)
			.AsNoTracking();

		if (!string.IsNullOrWhiteSpace(searchTerm))
		{
			var search = searchTerm.Trim().ToLower();
			query = query.Where(u =>
				u.Username.ToLower().Contains(search) ||
				u.Email.ToLower().Contains(search) ||
				(u.Profile != null && u.Profile.DisplayName != null && u.Profile.DisplayName.ToLower().Contains(search)));
		}

		if (roleId.HasValue)
		{
			query = query.Where(u => u.RoleId == roleId.Value);
		}

		if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<UserStatus>(status, true, out var parsedStatus))
		{
			query = query.Where(u => u.Status == parsedStatus);
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
