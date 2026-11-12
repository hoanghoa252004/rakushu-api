using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rakushu.Api.Endpoints.Material.Transcript;

internal static class TranscriptSegmentEndpointGroupExtension
{
	internal static RouteGroupBuilder MapTranscriptEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/videos/{videoId:guid}/transcript")
			.WithTags("Transcript")
			.WithGroupName("material");
	}

	internal static RouteGroupBuilder MapTranscriptSegmentEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/videos/{videoId:guid}/transcript")
			.WithTags("TranscriptSegment")
			.WithGroupName("material");
	}

	internal static RouteGroupBuilder MapTranscriptSpeakerEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/videos/{videoId:guid}/transcript")
			.WithTags("TranscriptSpeaker")
			.WithGroupName("material");
	}
}
