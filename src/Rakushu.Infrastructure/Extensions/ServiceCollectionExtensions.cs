using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Rakushu.Application.Abstractions.Authentication;
using Rakushu.Infrastructure.Authentication;

namespace Rakushu.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddRakushuInfrastructure(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		// AUTHENTICATION & AUTHORIZATION
		services.AddAuthenticationServices(configuration);

		return services;
	}
}
