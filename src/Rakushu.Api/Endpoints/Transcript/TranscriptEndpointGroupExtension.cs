using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rakushu.Api.Endpoints.Transcript;

internal static class TranscriptEndpointGroupExtension
{
	internal static RouteGroupBuilder MapTranscriptEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/admin/transcripts")
			.WithTags("Transcript")
			.WithGroupName("admin")
			.AllowAnonymous();
	}
}
