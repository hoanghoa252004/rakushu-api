using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rakushu.Api.Endpoints.Learning.Series;

internal static class TranscriptSegmentEndpointGroupExtension
{
	internal static RouteGroupBuilder MapTranscriptSegmentEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/video/{videoId:guid}/transcript/segments")
			.WithTags("TranscriptSegment")
			.WithGroupName("admin")
			.AllowAnonymous();
	}
}
