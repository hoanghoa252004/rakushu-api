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
}
