using Microsoft.OpenApi.Models;

namespace Rakushu.Api.Extensions;

internal static class SwaggerExtension
{
	internal static IServiceCollection AddSwaggerDocs(this IServiceCollection services)
	{
		services.AddEndpointsApiExplorer();

		return services.AddSwaggerGen(options =>
		{
			options.SwaggerDoc("common", new OpenApiInfo
			{
				Title = "Common API",
				Version = "v1",
				Description = "AUTHENTICATION, PROFILE"
			});

			options.SwaggerDoc("admin", new OpenApiInfo
			{
				Title = "Admin API",
				Version = "v1",
				Description = "USER - ROLE - SUSCRIPTION PLAN"
			});

			options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
				Name = "Authorization",
				In = ParameterLocation.Header,
				Type = SecuritySchemeType.Http,
				Scheme = "bearer",
				BearerFormat = "JWT"
			});

			options.AddSecurityRequirement(new OpenApiSecurityRequirement
			{
				{
					new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference
						{
							Type = ReferenceType.SecurityScheme,
							Id = "Bearer"
						}
					},
					Array.Empty<string>()
				}
			});
		});
	}

	internal static IApplicationBuilder UseSwaggerDocs(this IApplicationBuilder app)
	{
		app.UseSwagger();

		app.UseSwaggerUI(options =>
		{
			options.SwaggerEndpoint("/swagger/v1/swagger.json", "All APIs (v1)");
			options.SwaggerEndpoint("/swagger/auth/swagger.json", "Authentication API");
			options.SwaggerEndpoint("/swagger/profile/swagger.json", "Profile API");
			options.SwaggerEndpoint("/swagger/admin/swagger.json", "Admin API");
		});

		return app;
	}
}
