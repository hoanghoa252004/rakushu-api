using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Domain.Entities.Configuration.SystemConfiguration;

public sealed class SystemConfiguration : Entity<SystemConfigurationId>
{
	public SystemConfigurationKey ConfigKey { get; private set; }
	public ConfigurationDataType DataType { get; private set; }
	public string ConfigValue { get; private set; } = null!;
	public string? Description { get; private set; }
	public int VersionInUsed { get; private set; }
	public DateTime UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	private readonly List<SystemConfigurationHistory.SystemConfigurationHistory> _histories = new();
	public IReadOnlyCollection<SystemConfigurationHistory.SystemConfigurationHistory> Histories => _histories.AsReadOnly();

	private SystemConfiguration() { }

	// Used only for seeding: there is no public POST API.
	public static Result<SystemConfiguration> Create(
		SystemConfigurationKey key,
		ConfigurationDataType dataType,
		string value,
		string? description,
		DateTime now)
	{
		if (!ConfigurationValueValidator.IsValid(dataType, value))
			return Result.Failure<SystemConfiguration>(SystemConfigurationErrors.InvalidValue(dataType));

		value = ConfigurationValueValidator.Normalize(dataType, value);

		var id = SystemConfigurationId.Create();
		var config = new SystemConfiguration
		{
			Id = id,
			ConfigKey = key,
			DataType = dataType,
			ConfigValue = value,
			Description = description,
			VersionInUsed = 1,
			UpdatedAt = now
		};

		config._histories.Add(SystemConfigurationHistory.SystemConfigurationHistory.Create(id, 1, value, true, now));
		return Result.Success(config);
	}

	/// <summary>Creates a new version holding the value and makes it the only active one.</summary>
	public Result UpdateValue(string newValue, DateTime now)
	{
		if (!ConfigurationValueValidator.IsValid(DataType, newValue))
			return Result.Failure(SystemConfigurationErrors.InvalidValue(DataType));

		newValue = ConfigurationValueValidator.Normalize(DataType, newValue);

		if (newValue == ConfigValue)
			return Result.Failure(SystemConfigurationErrors.SameValue);

		foreach (var history in _histories)
			history.SetActive(false);

		var nextVersion = (_histories.Count == 0 ? 0 : _histories.Max(h => h.Version)) + 1;
		_histories.Add(SystemConfigurationHistory.SystemConfigurationHistory.Create(Id, nextVersion, newValue, true, now));

		ConfigValue = newValue;
		VersionInUsed = nextVersion;
		UpdatedAt = now;
		return Result.Success();
	}

	/// <summary>Switches the active version to an existing one.</summary>
	public Result ActivateVersion(int version, DateTime now)
	{
		var target = _histories.FirstOrDefault(h => h.Version == version);
		if (target is null)
			return Result.Failure(SystemConfigurationErrors.VersionNotFound);

		if (target.IsActive)
			return Result.Failure(SystemConfigurationErrors.VersionAlreadyActive);

		foreach (var history in _histories)
			history.SetActive(history.Version == version);

		ConfigValue = target.ConfigValue;
		VersionInUsed = version;
		UpdatedAt = now;
		return Result.Success();
	}
}
