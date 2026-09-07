using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Infrastructure.Clock;

namespace Rakushu.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddRakushuInfrastructure(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		// AUTHENTICATION & AUTHORIZATION
		services.AddAuthenticationServices(configuration);

		// CLOCK
		services.AddSingleton<ISystemClock, SystemClock>();
		return services;
	}
}
