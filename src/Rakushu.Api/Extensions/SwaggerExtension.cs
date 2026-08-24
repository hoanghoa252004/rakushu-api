using Microsoft.OpenApi.Models;

namespace Rakushu.Api.Extensions;

internal static class SwaggerExtension
{
	internal static IServiceCollection AddSwaggerDocs(this IServiceCollection services)
	{
		services.AddEndpointsApiExplorer();

		return services.AddSwaggerGen(options =>
		{
			options.SwaggerDoc("v1", new OpenApiInfo
			{
				Title = "Rakushu API",
				Version = "v1",
				Description = "Rakushu Japanese Learning API - Authentication, Profile, Admin User Management & Core Services"
			});

			options.SwaggerDoc("auth", new OpenApiInfo
			{
				Title = "Authentication API",
				Version = "v1",
				Description = "Endpoints related to Authentication: Register, Login, Logout, Change Password, Refresh Token"
			});

			options.SwaggerDoc("profile", new OpenApiInfo
			{
				Title = "Profile API",
				Version = "v1",
				Description = "Endpoints related to Profile Management"
			});

			options.SwaggerDoc("admin", new OpenApiInfo
			{
				Title = "Admin API",
				Version = "v1",
				Description = "Endpoints related to Admin User & Role Management"
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
