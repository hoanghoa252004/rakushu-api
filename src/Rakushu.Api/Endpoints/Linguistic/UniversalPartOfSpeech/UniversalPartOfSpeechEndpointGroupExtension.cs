using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rakushu.Api.Endpoints.Linguistic.UniversalPartOfSpeech;

internal static class UniversalPartOfSpeechEndpointGroupExtension
{
	internal static RouteGroupBuilder MapUniversalPartOfSpeechEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/admin/universal-part-of-speech")
			.WithTags("UniversalPartOfSpeech")
			.WithGroupName("admin")
			.AllowAnonymous();
	}
}
