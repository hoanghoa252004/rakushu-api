namespace Rakushu.Api.Endpoints.Learning.Tracking;

internal static class VideoTrackingEndpointGroupExtension
{
	internal static RouteGroupBuilder MapLearningVideoTrackingEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/learning/videos")
			.WithTags("Watch History & Progress")
			.WithGroupName("learning");
	}

	internal static RouteGroupBuilder MapLearningTrackingAnalyticsEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/learning/tracking")
			.WithTags("Immersion Analytics")
			.WithGroupName("learning");
	}
}
