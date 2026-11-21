using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Persistence.Repositories;

public sealed class UserRepository : BaseRepository<User, UserId>, IUserRepository
{
	public UserRepository(RakushuDbContext context) : base(context)
	{
	}

	public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
	{
		return await _context.Users
			.Include(u => u.Role)
			.Include(u => u.RefreshTokens)
			.Include(u => u.EmailVerificationTokens)
			.SingleOrDefaultAsync(u => u.Email.ToLower() == email.ToLower(), cancellationToken);
	}

	public async Task<User?> GetProfileByEmailAsync(string email, CancellationToken cancellationToken = default)
	{
		return await _context.Users
			.AsNoTracking()
			.Include(u => u.Role)
			.Include(u => u.Profile)
				.ThenInclude(p => p!.Level)
			.SingleOrDefaultAsync(u => u.Email.ToLower() == email.ToLower(), cancellationToken);
	}

	public async Task<User?> GetProfileByIdAsync(UserId id, CancellationToken cancellationToken = default)
	{
		return await _context.Users
			.AsNoTracking()
			.Include(u => u.Role)
			.Include(u => u.Profile)
				.ThenInclude(p => p!.Level)
			.Include(u => u.Profile)
				.ThenInclude(p => p!.Interests)
					.ThenInclude(i => i.ContentCategory)
			.AsSplitQuery()
			.SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
	}

	public async Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
	{
		return await _context.Users
			.Include(u => u.Role)
			.Include(u => u.RefreshTokens)
			.SingleOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.TokenHash== refreshToken), cancellationToken);
	}
	public override async Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default)
	{
		return await _context.Users
			.Include(u => u.Role)
			.Include(u => u.RefreshTokens)

			.Include(u => u.Subscriptions)
				.ThenInclude(s => s.Plan)

			.Include(u => u.Subscriptions)
				.ThenInclude(s => s.SubscriptionUsages)
					.ThenInclude(su => su.Feature)

			.Include(u => u.Payments)

			.Include(u => u.Profile)
				.ThenInclude(p => p.Interests)
					.ThenInclude(i => i.ContentCategory)

			.Include(u => u.Profile)
				.ThenInclude(p => p.Level)

			.AsSplitQuery()
			.SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
	}

	public async Task<IEnumerable<User>> GetByRoleIdAsync(RoleId roleId, CancellationToken cancellationToken = default)
	{
		return await _context.Users
			.Include(u => u.RefreshTokens)
			.Where(u => u.RoleId == roleId)
			.ToListAsync(cancellationToken);
	}

	public async Task<(IReadOnlyList<User> Items, int TotalCount)> GetPagedAsync(
		Guid? roleId,
		UserStatus? status,
		string? searchTerm,
		int pageNumber,
		int pageSize,
		CancellationToken cancellationToken = default)
	{
		var query = _context.Users
			.AsNoTracking()
			.Where(u => u.Role.Code != RoleCodes.SystemAdministrator);

		if (roleId.HasValue)
			query = query.Where(u => u.RoleId.Value == roleId.Value);

		if (status.HasValue)
			query = query.Where(u => u.Status == status.Value);

		if (!string.IsNullOrWhiteSpace(searchTerm))
		{
			var key = searchTerm.Trim().ToLower();
			query = query.Where(u => u.FullName.ToLower().Contains(key)
				|| u.Email.ToLower().Contains(key)
				|| u.Role.Name.ToLower().Contains(key));
		}

		var total = await query.CountAsync(cancellationToken);

		var items = await query
			.OrderBy(u => u.UpdatedAt)
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.Include(u => u.Role)
			.Include(u => u.Profile)
			.AsSplitQuery()
			.ToListAsync(cancellationToken);

		return (items, total);
	}

	public override async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return await _context.Users
			.Include(u => u.Role)
			.Include(u => u.RefreshTokens)

			.Include(u => u.Subscriptions)
				.ThenInclude(s => s.Plan)

			.Include(u => u.Subscriptions)
				.ThenInclude(s => s.SubscriptionUsages)

			.Include(u => u.Payments)

			.Include(u => u.Profile)
				.ThenInclude(p => p.Interests)
					.ThenInclude(i => i.ContentCategory)

			.Include(u => u.Profile)
				.ThenInclude(p => p.Level)
			.ToListAsync(cancellationToken);
	}
}
