namespace Rakushu.Api.Endpoints.Shadowing.Speaker;

internal static class SpeakerEndpointGroupExtension
{
	internal static RouteGroupBuilder MapSpeakerEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/speakers")
			.WithTags("Speaker")
			.WithGroupName("shadowing");
	}
}