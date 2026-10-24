using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rakushu.Application.Abstractions.Infrastructure.AiService;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Abstractions.Infrastructure.Payment;
using Rakushu.Application.Abstractions.Infrastructure.PaymentGateway;
using Rakushu.Infrastructure.Authentication;
using Rakushu.Infrastructure.Clock;
using Rakushu.Infrastructure.Extensions.Options;
using Rakushu.Infrastructure.Payment;
using Rakushu.Infrastructure.Payment.VnPay;

namespace Rakushu.Infrastructure.Extensions;

public static class InfrastructureServiceCollectionExtensions
{
	public static IServiceCollection AddRakushuInfrastructureForApi(
		this IServiceCollection services,
		IConfiguration configuration,
		IHostEnvironment environment)
	{
		services.AddAuthenticationServices(configuration);

		services.AddRakushuInfrastructureForCommon(configuration, environment);

		return services;
	}

	public static IServiceCollection AddRakushuInfrastructureForWorker(
		this IServiceCollection services,
		IConfiguration configuration,
		IHostEnvironment environment)
	{
		services.AddScoped<ICurrentUserContext, WorkerCurrentUserContext>();

		services.AddRakushuInfrastructureForCommon(configuration, environment);

		return services;
	}

	public static IServiceCollection AddRakushuInfrastructureForCommon(
		this IServiceCollection services,
		IConfiguration configuration,
		IHostEnvironment environment)
	{
		// VERIFICATION
		services.AddScoped<IPasswordHasher, PasswordHasher>();
		services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
		services.AddScoped<IVerificationCodeHasher, VerificationCodeHasher>();

		// AWS SERVICES
		services.AddAwsServices(configuration, environment);

		// CLOCK
		services.AddSingleton<ISystemClock, SystemClock>();

		// PAYMENT
		services.Configure<VnPaySettings>(configuration.GetSection(VnPaySettings.ConfigurationSection));
		services.AddScoped<IPaymentService, VnPayService>();

		// PAYMENT GATEWAY
		services.AddScoped<VnPayService>();
		services.AddScoped<IPaymentGatewayFactory, PaymentGatewayFactory>();

		// AI SERVICE CLIENT
		services.AddHttpClient<IAiServiceClient, Rakushu.Infrastructure.AiService.AiServiceClient>();
		services.AddHttpClient<IQuizContentGenerator, Rakushu.Infrastructure.AiService.OllamaQuizContentGenerator>();

		return services;
	}
}
