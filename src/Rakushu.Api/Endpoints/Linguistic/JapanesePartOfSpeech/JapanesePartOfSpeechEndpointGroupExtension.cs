using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rakushu.Api.Endpoints.Linguistic.JapanesePartOfSpeech;

internal static class JapanesePartOfSpeechEndpointGroupExtension
{
	internal static RouteGroupBuilder MapJapanesePartOfSpeechEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/admin/japanese-part-of-speech")
			.WithTags("JapanesePartOfSpeech")
			.WithGroupName("admin")
			.AllowAnonymous();
	}
}
