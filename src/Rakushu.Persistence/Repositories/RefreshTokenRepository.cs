using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities;
using Rakushu.Domain.Repositories;
using Rakushu.Persistence.DbContext;

namespace Rakushu.Persistence.Repositories;

public sealed class RefreshTokenRepository : Repository<RefreshToken, Guid>, IRefreshTokenRepository
{
	public RefreshTokenRepository(RakushuDbContext context) : base(context)
	{
	}

	public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
	{
		return await _dbSet
			.Include(t => t.User)
				.ThenInclude(u => u.Role)
			.Include(t => t.User)
				.ThenInclude(u => u.Profile)
			.FirstOrDefaultAsync(t => t.Token == token, cancellationToken);
	}

	public async Task RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken = default)
	{
		var activeTokens = await _dbSet
			.Where(t => t.UserId == userId && !t.IsRevoked)
			.ToListAsync(cancellationToken);

		foreach (var token in activeTokens)
		{
			token.Revoke();
		}
	}
}
