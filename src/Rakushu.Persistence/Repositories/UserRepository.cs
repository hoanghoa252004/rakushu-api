using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.User.RefreshToken;
using Rakushu.Domain.Entities.User.ValueObjects.Email;

namespace Rakushu.Persistence.Repositories;

public sealed class UserRepository : BaseRepository<User, UserId>, IUserRepository
{
	public UserRepository(RakushuDbContext context) : base(context)
	{
	}

	public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
	{
		var normalizedEmail = Email.NormalizeEmail(email);

		return await _context.Users
			.Include(u => u.Role)
			.Include(u => u.RefreshTokens)
			.Include(u => u.EmailVerificationTokens)
			.SingleOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);
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
			.SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
	}

	public async Task<User?> GetByIdWithSubscriptionsAsync(UserId id, CancellationToken cancellationToken = default)
	{
		return await _context.Users
			.Include(u => u.Role)
			.Include(u => u.Subscriptions)
				.ThenInclude(s => s.SubscriptionUsages)
			.Include(u => u.Subscriptions)
				.ThenInclude(s => s.Plan)
					.ThenInclude(p => p.PlanEntitlements)
			.SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
	}

	public async Task<User?> GetBySubscriptionIdAsync(Domain.Entities.User.Subscription.SubscriptionId subscriptionId, CancellationToken cancellationToken = default)
	{
		return await _context.Users
			.Include(u => u.Subscriptions)
				.ThenInclude(s => s.SubscriptionUsages)
			.Include(u => u.Subscriptions)
				.ThenInclude(s => s.Plan)
					.ThenInclude(p => p.PlanEntitlements)
			.SingleOrDefaultAsync(u => u.Subscriptions.Any(s => s.Id == subscriptionId), cancellationToken);
	}
}
