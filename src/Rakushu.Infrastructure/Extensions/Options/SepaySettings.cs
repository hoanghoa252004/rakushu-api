namespace Rakushu.Infrastructure.Extensions.Options;

public sealed class SepaySettings
{
	public const string ConfigurationSection = nameof(SepaySettings);
	public string AccountNumber { get; set; } = string.Empty;
	public string Bank { get; set; } = string.Empty;
	public string ApiKey { get; set; } = string.Empty;
	public string SecretKey { get; set; } = string.Empty;
}
