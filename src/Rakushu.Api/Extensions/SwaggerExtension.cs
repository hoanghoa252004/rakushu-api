using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Rakushu.Domain.Entities.User.Subscription;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Rakushu.Api.Extensions;

internal static class SwaggerExtension
{
	internal static IServiceCollection AddSwaggerDocs(this IServiceCollection services)
	{
		services.AddEndpointsApiExplorer();

		return services.AddSwaggerGen(options =>
		{
			options.SchemaFilter<EnumSchemaFilter>();

			options.SwaggerDoc("auth", new OpenApiInfo
			{
				Title = "Authentication API",
				Version = "v1",
				Description = "AUTHENTICATION"
			});

			options.SwaggerDoc("user", new OpenApiInfo
			{
				Title = "User API",
				Version = "v1",
				Description = "ROLE - USER - PROFILE"
			});

			options.SwaggerDoc("storage", new OpenApiInfo
			{
				Title = "Storage API",
				Version = "v1",
				Description = "STORAGE"
			}); 

			options.SwaggerDoc("subscription", new OpenApiInfo
			{
				Title = "Subscription API",
				Version = "v1",
				Description = "SUBSCRIPTION"
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
			options.SwaggerEndpoint("/swagger/auth/swagger.json", "Authentication API");
			options.SwaggerEndpoint("/swagger/user/swagger.json", "User API");
			options.SwaggerEndpoint("/swagger/storage/swagger.json", "Storage API");
			options.SwaggerEndpoint("/swagger/subscription/swagger.json", "Subscription API");

		});

		return app;
	}
}

internal sealed class EnumSchemaFilter : ISchemaFilter
{
	public void Apply(OpenApiSchema schema, SchemaFilterContext context)
	{
		var type = Nullable.GetUnderlyingType(context.Type) ?? context.Type;

		if (!type.IsEnum)
			return;

		schema.Type = "string";
		schema.Format = null;
		schema.Enum = Enum.GetNames(type)
			.Select(name => new OpenApiString(name))
			.Cast<IOpenApiAny>()
			.ToList();
	}
}