using Rakushu.Domain.Entities.Configuration.SystemConfiguration;

namespace Rakushu.Application.Abstractions.Infrastructure.Configuration;

/// <summary>Reads the active version of database-managed configurations by key.</summary>
public interface ISystemConfigurationProvider
{
	string GetString(SystemConfigurationKey key);
	int GetInt(SystemConfigurationKey key);
	bool GetBool(SystemConfigurationKey key);
	decimal GetDecimal(SystemConfigurationKey key);

	Task<string> GetStringAsync(SystemConfigurationKey key, CancellationToken cancellationToken = default);
	Task<int> GetIntAsync(SystemConfigurationKey key, CancellationToken cancellationToken = default);
	Task<bool> GetBoolAsync(SystemConfigurationKey key, CancellationToken cancellationToken = default);
	Task<decimal> GetDecimalAsync(SystemConfigurationKey key, CancellationToken cancellationToken = default);
}
