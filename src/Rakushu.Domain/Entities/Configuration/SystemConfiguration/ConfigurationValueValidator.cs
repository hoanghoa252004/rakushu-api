using System.Globalization;

namespace Rakushu.Domain.Entities.Configuration.SystemConfiguration;

public static class ConfigurationValueValidator
{
	public static bool IsValid(ConfigurationDataType dataType, string? value)
	{
		if (value is null)
			return false;

		return dataType switch
		{
			ConfigurationDataType.String => true,
			ConfigurationDataType.Integer => int.TryParse(value.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out _),
			ConfigurationDataType.Boolean => bool.TryParse(value.Trim(), out _),
			ConfigurationDataType.Decimal => decimal.TryParse(value.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out _),
			_ => false
		};
	}

	/// <summary>Strings are kept as is, other types are trimmed.</summary>
	public static string Normalize(ConfigurationDataType dataType, string value)
		=> dataType == ConfigurationDataType.String ? value : value.Trim();
}
