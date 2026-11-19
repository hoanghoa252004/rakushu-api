using Rakushu.Domain.Common.Contract;

namespace Rakushu.Domain.Entities.Configuration.SystemConfiguration;

public interface ISystemConfigurationRepository : IBaseRepository<SystemConfiguration, SystemConfigurationId>
{
	Task<SystemConfiguration?> GetByKeyAsync(SystemConfigurationKey key, CancellationToken cancellationToken = default);
}
