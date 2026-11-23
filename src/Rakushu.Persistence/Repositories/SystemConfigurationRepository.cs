using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Configuration.SystemConfiguration;

namespace Rakushu.Persistence.Repositories;

public sealed class SystemConfigurationRepository : BaseRepository<SystemConfiguration, SystemConfigurationId>, ISystemConfigurationRepository
{
	public SystemConfigurationRepository(RakushuDbContext context) : base(context)
	{
	}

	public override async Task<SystemConfiguration?> GetByIdAsync(SystemConfigurationId id, CancellationToken cancellationToken = default)
	{
		return await _context.SystemConfigurations
			.Include(c => c.Histories)
			.SingleOrDefaultAsync(c => c.Id == id, cancellationToken);
	}

	public async Task<SystemConfiguration?> GetByKeyAsync(SystemConfigurationKey key, CancellationToken cancellationToken = default)
	{
		return await _context.SystemConfigurations
			.Include(c => c.Histories)
			.SingleOrDefaultAsync(c => c.ConfigKey == key, cancellationToken);
	}

	public override async Task<IEnumerable<SystemConfiguration>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return await _context.SystemConfigurations
			.Include(c => c.Histories)
			.OrderBy(c => c.ConfigKey)
			.ToListAsync(cancellationToken);
	}
}
