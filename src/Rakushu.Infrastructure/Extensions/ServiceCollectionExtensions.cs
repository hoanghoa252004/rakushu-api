using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Abstractions.Infrastructure.Payment;
using Rakushu.Application.Abstractions.Infrastructure.PaymentGateway;
using Rakushu.Infrastructure.Clock;
using Rakushu.Infrastructure.Extensions.Options;
using Rakushu.Infrastructure.Payment;
using Rakushu.Infrastructure.Payment.VnPay;

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

		// PAYMENT
		services.Configure<VnPaySettings>(configuration.GetSection(VnPaySettings.ConfigurationSection));
		services.AddScoped<IPaymentService, VnPayService>();

		// PAYMENT GATEWAY
		services.AddScoped<VnPayService>();
		services.AddScoped<IPaymentGatewayFactory, PaymentGatewayFactory>();

		return services;
	}
}
