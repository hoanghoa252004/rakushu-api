using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Knowledge.KnowledgeCollection.GetKnowledgeCollectionById;
using Rakushu.Application.Usecases.Knowledge.KnowledgeCollection.GetKnowledgeCollections;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Knowledge.KnowledgeCollection.GetKnowledgeCollections;

internal sealed class GetKnowledgeCollections : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapKnowledgeCollectionEndpoints()
			// 1. Endpoint
			.MapGet("/", async (
				ISender sender,
				CancellationToken cancellationToken,
				int pageNumber = 1,
				int pageSize = 10,
				string? searchTerm = null,
				string? active = null,
				string? type = null,
				string? scope = null
				) =>
			{
				var query = new GetKnowledgeCollectionsQuery(pageNumber, pageSize, searchTerm, active, type, scope);

				var result = await sender.Send(query, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("GetKnowledgeCollections")
			.WithSummary("Learner (active only), Curator")
			.WithDescription("Retrieves knowledge collections. Filters: searchTerm, active (true/false), type (Word, Collocation, Grammar, Terminology, ProperNoun, Mixed). Learners only receive active records.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner, RoleCodes.LinguisticCurator))
			// 4. Response
			.Produces<PaginatedList<KnowledgeCollectionDto>>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
