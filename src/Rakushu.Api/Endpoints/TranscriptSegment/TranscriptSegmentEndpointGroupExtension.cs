using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rakushu.Api.Endpoints.TranscriptSegment;

internal static class TranscriptSegmentEndpointGroupExtension
{
	internal static RouteGroupBuilder MapTranscriptSegmentEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/admin/transcript-segments")
			.WithTags("TranscriptSegment")
			.WithGroupName("admin")
			.AllowAnonymous();
	}
}
