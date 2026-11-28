using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Identity.User.PasswordResetCode;

namespace Rakushu.Persistence.Repositories;

public sealed class PasswordResetCodeRepository : BaseRepository<PasswordResetCode, PasswordResetCodeId>, IPasswordResetCodeRepository
{
	public PasswordResetCodeRepository(RakushuDbContext context) : base(context)
	{
	}

	public async Task<PasswordResetCode?> GetActiveByUserIdAsync(UserId userId, DateTimeOffset now, CancellationToken cancellationToken = default)
	{
		return await _dbSet
			.Where(x => x.UserId == userId && !x.IsUsed && !x.IsExpired && x.ExpiresAt > now)
			.OrderByDescending(x => x.CreatedAt)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<PasswordResetCode?> GetByUserIdAndCodeHashAsync(UserId userId, string codeHash, CancellationToken cancellationToken = default)
	{
		return await _dbSet
			.Where(x => x.UserId == userId && x.CodeHash == codeHash)
			.OrderByDescending(x => x.CreatedAt)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<PasswordResetCode>> GetNotYetFlaggedExpiredAsync(DateTimeOffset now, CancellationToken cancellationToken = default)
	{
		return await _dbSet
			.Where(x => !x.IsExpired && x.ExpiresAt <= now)
			.ToListAsync(cancellationToken);
	}
}
