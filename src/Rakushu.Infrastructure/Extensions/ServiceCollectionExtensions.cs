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
		return services;
	}
}
