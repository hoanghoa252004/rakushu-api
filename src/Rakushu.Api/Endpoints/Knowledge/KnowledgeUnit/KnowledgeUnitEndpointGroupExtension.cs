using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rakushu.Api.Endpoints.Knowledge.KnowledgeUnit;

internal static class KnowledgeUnitEndpointGroupExtension
{
	internal static RouteGroupBuilder MapKnowledgeUnitEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/knowledge-units")
			.WithTags("KnowledgeUnit")
			.WithGroupName("knowledge");
	}
}
