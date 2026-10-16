using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rakushu.Api.Endpoints.Knowledge.JapanesePartOfSpeech;

internal static class JapanesePartOfSpeechEndpointGroupExtension
{
	internal static RouteGroupBuilder MapJapanesePartOfSpeechEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/japanese-parts-of-speech")
			.WithTags("JapanesePartOfSpeech")
			.WithGroupName("knowledge");
	}
}
