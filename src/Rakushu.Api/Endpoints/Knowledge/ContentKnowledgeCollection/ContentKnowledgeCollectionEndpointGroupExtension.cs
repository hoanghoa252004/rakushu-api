namespace Rakushu.Api.Endpoints.Knowledge.ContentKnowledgeCollection;

internal static class ContentKnowledgeCollectionEndpointGroupExtension
{
	internal static RouteGroupBuilder MapContentKnowledgeCollectionEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/content-knowledge-collections")
			.WithTags("ContentKnowledgeCollection")
			.WithGroupName("knowledge");
	}
}
