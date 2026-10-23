using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Knowledge.KnowledgeUnit.GetKnowledgeUnitById;
using Rakushu.Application.Usecases.Knowledge.KnowledgeUnit.GetKnowledgeUnits;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Knowledge.KnowledgeUnit.GetKnowledgeUnits;

internal sealed class GetKnowledgeUnits : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapKnowledgeUnitEndpoints()
			// 1. Endpoint
			.MapGet("/", async (
				ISender sender,
				CancellationToken cancellationToken,
				Guid? knowledgeCollectionId = null,
				int pageNumber = 1,
				int pageSize = 10,
				string? searchTerm = null,
				string? active = null,
				string? type = null
				) =>
			{
				var query = new GetKnowledgeUnitsQuery(pageNumber, pageSize, knowledgeCollectionId, searchTerm, active, type);

				var result = await sender.Send(query, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("GetKnowledgeUnits")
			.WithSummary("Learner (active only), Curator")
			.WithDescription("Retrieves knowledge units. Filters: knowledgeCollectionId, searchTerm, active (true/false), type (Word, Collocation, Grammar, Terminology, ProperNoun). Learners only receive active records.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner, RoleCodes.LinguisticCurator))
			// 4. Response
			.Produces<PaginatedList<KnowledgeUnitDto>>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
