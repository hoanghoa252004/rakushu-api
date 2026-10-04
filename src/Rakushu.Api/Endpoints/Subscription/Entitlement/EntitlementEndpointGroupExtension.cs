namespace Rakushu.Api.Endpoints.Subscription.Entitlement;

internal static class EntitlementEndpointGroupExtension
{
	internal static RouteGroupBuilder MapEntitlementEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/plans/{planId:guid}/entitlements")
			.WithTags("Entitlement")
			.WithGroupName("subscription");
	}
}
