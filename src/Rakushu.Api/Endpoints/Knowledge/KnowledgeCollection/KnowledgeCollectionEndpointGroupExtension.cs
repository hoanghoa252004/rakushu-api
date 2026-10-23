using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rakushu.Api.Endpoints.Knowledge.KnowledgeCollection;

internal static class KnowledgeCollectionEndpointGroupExtension
{
	internal static RouteGroupBuilder MapKnowledgeCollectionEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/knowledge-collections")
			.WithTags("KnowledgeCollection")
			.WithGroupName("knowledge");
	}
}
