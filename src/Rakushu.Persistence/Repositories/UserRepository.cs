using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.Role;

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

			.SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
	}

	public async Task<IEnumerable<User>> GetByRoleIdAsync(RoleId roleId, CancellationToken cancellationToken = default)
	{
		return await _context.Users
			.Include(u => u.RefreshTokens)
			.Where(u => u.RoleId == roleId)
			.ToListAsync(cancellationToken);
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
