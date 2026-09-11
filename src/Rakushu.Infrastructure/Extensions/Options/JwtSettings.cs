namespace Rakushu.Infrastructure.Extensions.Options;

public sealed class JwtSettings
{
	public const string ConfigurationSection = nameof(JwtSettings);
	public string SecretKey { get; set; } = string.Empty;
	public string Issuer { get; set; } = string.Empty;
	public string Audience { get; set; } = string.Empty;
	public int AccessTokenExpirationMinutes { get; set; } = 60;
	public int RefreshTokenExpirationDays { get; set; } = 7;
	public int EmailVerificationTokenExipationMinutes { get; set; } = 5;
}
