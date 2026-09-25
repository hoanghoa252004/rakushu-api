namespace Rakushu.Api.Endpoints.PlanEntitlement;

internal static class PlanEntitlementEndpointGroupExtension
{
	internal static RouteGroupBuilder MapPlanEntitlementEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/plans/{planId:guid}/entitlements")
			.WithTags("Entitlement")
			.WithGroupName("subscription");
	}
}
