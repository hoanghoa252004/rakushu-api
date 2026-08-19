using Rakushu.Api.Common;

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

		return services;
	}
}
