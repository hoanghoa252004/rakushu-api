using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Configuration.SystemConfiguration;

public static class SystemConfigurationErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"SYSTEM_CONFIGURATION.NOT_FOUND", "The specified system configuration was not found.");

	public static Error KeyNotFound(SystemConfigurationKey key) => Error.NotFound(
		"SYSTEM_CONFIGURATION.KEY_NOT_FOUND", $"The system configuration '{key}' was not found.");

	public static Error InvalidValue(ConfigurationDataType dataType) => Error.Validation(
		"SYSTEM_CONFIGURATION.INVALID_VALUE", $"The config value is not a valid {dataType}.");

	public static readonly Error VersionNotFound = Error.NotFound(
		"SYSTEM_CONFIGURATION.VERSION_NOT_FOUND", "The specified version does not exist.");

	public static readonly Error NothingToUpdate = Error.Validation(
		"SYSTEM_CONFIGURATION.NOTHING_TO_UPDATE", "Either ConfigValue (create a new version) or Version (switch active version) is required.");

	public static readonly Error BothValueAndVersion = Error.Validation(
		"SYSTEM_CONFIGURATION.BOTH_VALUE_AND_VERSION", "Provide only one of ConfigValue or Version.");

	public static readonly Error SameValue = Error.Conflict(
		"SYSTEM_CONFIGURATION.SAME_VALUE", "The config value is identical to the one currently in use.");

	public static readonly Error VersionAlreadyActive = Error.Conflict(
		"SYSTEM_CONFIGURATION.VERSION_ALREADY_ACTIVE", "The specified version is already in use.");

	public static Error ValueExistsInVersion(int version) => Error.Conflict(
		"SYSTEM_CONFIGURATION.VALUE_EXISTS_IN_VERSION",
		$"A configuration with this value already exists in version {version}. Please switch to version {version} instead.");
}
