using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
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
				Description = "APIs for authentication and token management."
			});

			options.SwaggerDoc("user", new OpenApiInfo
			{
				Title = "User API",
				Version = "v1",
				Description = "APIs for user accounts, profiles, and roles."
			});

			options.SwaggerDoc("storage", new OpenApiInfo
			{
				Title = "Storage API",
				Version = "v1",
				Description = "APIs for file storage and media management."
			});

			options.SwaggerDoc("subscription", new OpenApiInfo
			{
				Title = "Subscription API",
				Version = "v1",
				Description = "APIs for subscription & payment."
			});

			options.SwaggerDoc("material", new OpenApiInfo
			{
				Title = "Material API",
				Version = "v1",
				Description = "APIs for learning content."
			});

			options.SwaggerDoc("knowledge", new OpenApiInfo
			{
				Title = "Knowlege API",
				Version = "v1",
				Description = "APIs for knowledge."
			});

			options.SwaggerDoc("shadowing", new OpenApiInfo
			{
				Title = "Shadowing API",
				Version = "v1",
				Description = "APIs for shadowing."
			});

			options.SwaggerDoc("srs", new OpenApiInfo
			{
				Title = "SRS API",
				Version = "v1",
				Description = "APIs for Spaced Repetition System (SRS) including Vocabulary Items, Quizzes, and Flashcard Sessions."
			});

			options.SwaggerDoc("curation", new OpenApiInfo
			{
				Title = "Curation API",
				Version = "v1",
				Description = "APIs for Learner Issue Reporting and Curator Triage."
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
			options.SwaggerEndpoint("/swagger/material/swagger.json", "Material API");
			options.SwaggerEndpoint("/swagger/knowledge/swagger.json", "Knowledge API");
			options.SwaggerEndpoint("/swagger/shadowing/swagger.json", "Shadowing API");
			options.SwaggerEndpoint("/swagger/srs/swagger.json", "SRS API");
			options.SwaggerEndpoint("/swagger/curation/swagger.json", "Curation API");
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
