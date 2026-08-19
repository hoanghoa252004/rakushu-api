using System.Runtime.CompilerServices;

namespace Rakushu.Api.Extensions;

internal static class SwaggerExtension
{
	internal static IServiceCollection AddSwaggerDocs(this IServiceCollection services)
	{
		return services.AddSwaggerGen(options =>
		{

			options.SwaggerDoc("auth", new Microsoft.OpenApi.Models.OpenApiInfo()
			{
				Title = "Authentication API",
				Version = "v1",
				Description = "Endpoints related to Authentication: Register, Login,..."
			});
		});
	}

	internal static IApplicationBuilder UseSwaggerDocs(this IApplicationBuilder app)
	{
		app.UseSwagger();

		app.UseSwaggerUI(options =>
		{
			options.SwaggerEndpoint("/swagger/auth/swagger.json", "Authentication API");
		});

		return app;
	}
}
