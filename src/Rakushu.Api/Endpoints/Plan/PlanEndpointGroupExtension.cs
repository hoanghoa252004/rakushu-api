namespace Rakushu.Api.Endpoints.Plan;

internal static class PlanEndpointGroupExtension
{
	internal static RouteGroupBuilder MapPlanEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/plans")
			.WithTags("Plan")
			.WithGroupName("subscription");
	}
}
