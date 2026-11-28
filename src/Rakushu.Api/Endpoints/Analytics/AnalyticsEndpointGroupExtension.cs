namespace Rakushu.Api.Endpoints.Analytics;

internal static class AnalyticsEndpointGroupExtension
{
	internal static RouteGroupBuilder MapLearnerAnalyticsEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/analytics/learner")
			.WithTags("Learner Analytics")
			.WithGroupName("analytics");
	}

	internal static RouteGroupBuilder MapShadowingAnalyticsEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/analytics/shadowing")
			.WithTags("Shadowing Analytics")
			.WithGroupName("analytics");
	}

	internal static RouteGroupBuilder MapCuratorAnalyticsEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/analytics/curator")
			.WithTags("Curator Analytics")
			.WithGroupName("analytics");
	}

	internal static RouteGroupBuilder MapFinanceAnalyticsEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/analytics/finance")
			.WithTags("Finance Analytics")
			.WithGroupName("analytics");
	}

	internal static RouteGroupBuilder MapAdminAnalyticsEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/analytics/admin")
			.WithTags("Admin Analytics")
			.WithGroupName("analytics");
	}
}
