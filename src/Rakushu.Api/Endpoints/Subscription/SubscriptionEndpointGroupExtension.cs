namespace Rakushu.Api.Endpoints.Subscription;

internal static class SubscriptionEndpointGroupExtension
{
	internal static RouteGroupBuilder MapSubscriptionEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/subscriptions")
			.WithTags("Subscription")
			.WithGroupName("subscription");
	}
}
