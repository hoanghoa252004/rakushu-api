namespace Rakushu.Infrastructure.Extensions.Options;

public sealed class PaymentSettings
{
	public const string ConfigurationSection = nameof(PaymentSettings);
	public int TimeoutInMinutes { get; set; } = 15;
}
