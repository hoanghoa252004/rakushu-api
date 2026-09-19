namespace Rakushu.Api.Endpoints.Feature;

internal static class FeatureEndpointGroupExtension
{
	internal static RouteGroupBuilder MapFeatureEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/features")
			.WithTags("Feature")
			.WithGroupName("subscription");
	}
}
