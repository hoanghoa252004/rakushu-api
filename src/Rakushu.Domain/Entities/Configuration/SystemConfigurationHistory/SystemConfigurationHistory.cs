using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Configuration.SystemConfiguration;

namespace Rakushu.Domain.Entities.Configuration.SystemConfigurationHistory;

public sealed class SystemConfigurationHistory : Entity<SystemConfigurationHistoryId>
{
	public int Version { get; private set; }
	public SystemConfigurationId ConfigurationId { get; private set; } = null!;
	public string ConfigValue { get; private set; } = null!;
	public bool IsActive { get; private set; }
	public DateTime CreatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	public SystemConfiguration.SystemConfiguration Configuration { get; private set; } = null!;

	private SystemConfigurationHistory() { }

	internal static SystemConfigurationHistory Create(
		SystemConfigurationId configurationId,
		int version,
		string configValue,
		bool isActive,
		DateTime now)
	{
		return new SystemConfigurationHistory
		{
			Id = SystemConfigurationHistoryId.Create(),
			ConfigurationId = configurationId,
			Version = version,
			ConfigValue = configValue,
			IsActive = isActive,
			CreatedAt = now
		};
	}

	internal void SetActive(bool isActive) => IsActive = isActive;
}
