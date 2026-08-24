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
		var jwtSection = configuration.GetSection(JwtSettings.SectionName);
		services.Configure<JwtSettings>(jwtSection);

		var jwtSettings = jwtSection.Get<JwtSettings>() ?? new JwtSettings();

		// Services
		services.AddHttpContextAccessor();
		services.AddScoped<IPasswordHasher, PasswordHasher>();
		services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
		services.AddScoped<ICurrentUserContext, CurrentUserContext>();

		// JWT Authentication
		var secretKey = !string.IsNullOrWhiteSpace(jwtSettings.SecretKey)
			? jwtSettings.SecretKey
			: "RakushuDevSecretKeyChangeMeInProductionMinimum32Chars!";

		services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		})
		.AddJwtBearer(options =>
		{
			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ValidIssuer = jwtSettings.Issuer,
				ValidAudience = jwtSettings.Audience,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
				ClockSkew = TimeSpan.Zero
			};
		});

		services.AddAuthorization();

		return services;
	}
}
