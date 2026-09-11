using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Infrastructure.Authentication;
using Rakushu.Infrastructure.Extensions.Options;
using System.Text;

namespace Rakushu.Infrastructure.Extensions;

internal static class AuthenticationExtention
{
	internal static IServiceCollection AddAuthenticationServices(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		// Authentication:
		services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.ConfigurationSection));
		var jwtSettings = configuration.GetSection(nameof(JwtSettings));

		services.AddHttpContextAccessor();

		services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			//options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		})
		.AddJwtBearer(options =>
		{

			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ValidIssuer = jwtSettings[nameof(JwtSettings.Issuer)],
				ValidAudience = jwtSettings[nameof(JwtSettings.Audience)],
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings[nameof(JwtSettings.SecretKey)]!)),
				ClockSkew = TimeSpan.Zero
			};
		});

		services.AddAuthorization();


		services.AddScoped<ICurrentUserContext, CurrentUserContext>();
		services.AddScoped<IPasswordHasher, PasswordHasher>();
		services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
		services.AddScoped<IVerificationCodeHasher, VerificationCodeHasher>();
		return services;
	}
}
