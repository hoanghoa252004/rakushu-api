using Rakushu.Api.Common;
using System.Text.Json.Serialization;

namespace Rakushu.Api.Extensions;

public static class EndpointExtention
{
	public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
	{
		var endpoints = app.ServiceProvider.GetServices<IEndpoint>();

		foreach (var endpoint in endpoints)
		{
			endpoint.MapEndpoint(app);
		}

		return app;
	}
	public static IServiceCollection AddRakushuApi(
	this IServiceCollection services)
	{
		var assembly = typeof(EndpointExtention).Assembly;

		var endpointTypes = assembly.GetTypes()
			.Where(t => typeof(IEndpoint).IsAssignableFrom(t)
						&& t is { IsClass: true, IsAbstract: false }
			);

		foreach (var type in endpointTypes)
		{
			services.AddSingleton(typeof(IEndpoint), type);
		}

		services.ConfigureHttpJsonOptions(options =>
		{
			options.SerializerOptions.Converters.Add(
				new JsonStringEnumConverter());
		});

		services.AddCors(options =>
		{
			options.AddPolicy("Frontend", policy =>
			{
				policy
					.WithOrigins(
						"http://localhost:3000",
						"http://localhost:8000",
						"https://fptu-rakushu.com"
					)
					.AllowAnyHeader()
					.AllowAnyMethod();
			});
		});

		return services;
	}
}
