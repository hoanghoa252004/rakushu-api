using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Abstractions.Infrastructure.Email;
using Rakushu.Infrastructure.Clock;
using Rakushu.Infrastructure.Email;
using Rakushu.Infrastructure.Extensions.Options;

namespace Rakushu.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddRakushuInfrastructure(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		// AUTHENTICATION & AUTHORIZATION
		services.AddAuthenticationServices(configuration);

		// AWS SERVICES
		services.AddAwsServices(configuration);

		// CLOCK
		services.AddSingleton<ISystemClock, SystemClock>();

		// PAYMENT & SEPAY
		services.Configure<PaymentSettings>(configuration.GetSection(PaymentSettings.ConfigurationSection));
		services.Configure<SepaySettings>(configuration.GetSection(SepaySettings.ConfigurationSection));
		services.AddScoped<Rakushu.Application.Abstractions.Infrastructure.Payment.ISepayService, Rakushu.Infrastructure.Payment.SepayService>();
		services.AddHostedService<Rakushu.Infrastructure.Payment.PaymentExpirationBackgroundService>();

		return services;
	}
}
