using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities;
using Rakushu.Domain.Repositories;
using Rakushu.Persistence.DbContext;

namespace Rakushu.Persistence.Repositories;

public sealed class ProfileRepository : Repository<Profile, Guid>, IProfileRepository
{
	public ProfileRepository(RakushuDbContext context) : base(context)
	{
	}

	public async Task<Profile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
	{
		return await _dbSet
			.Include(p => p.User)
			.FirstOrDefaultAsync(p => p.Id == userId, cancellationToken);
	}
}
