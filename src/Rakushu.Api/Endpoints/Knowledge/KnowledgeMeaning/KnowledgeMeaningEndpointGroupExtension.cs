using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rakushu.Api.Endpoints.Knowledge.KnowledgeMeaning;

internal static class KnowledgeMeaningEndpointGroupExtension
{
	internal static RouteGroupBuilder MapKnowledgeMeaningEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/knowledge-meanings")
			.WithTags("KnowledgeMeaning")
			.WithGroupName("knowledge");
	}
}
