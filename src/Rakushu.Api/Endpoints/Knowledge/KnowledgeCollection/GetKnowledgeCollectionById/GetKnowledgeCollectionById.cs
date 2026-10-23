using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Knowledge.KnowledgeCollection.GetKnowledgeCollectionById;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Knowledge.KnowledgeCollection.GetKnowledgeCollectionById;

internal sealed class GetKnowledgeCollectionById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapKnowledgeCollectionEndpoints()
			// 1. Endpoint
			.MapGet("/{id:guid}", async (
				[FromRoute] Guid id,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var query = new GetKnowledgeCollectionByIdQuery(id);

				var result = await sender.Send(query, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("GetKnowledgeCollectionById")
			.WithSummary("Learner (active only), Curator")
			.WithDescription("Retrieves a specific knowledge collection by its ID.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner, RoleCodes.LinguisticCurator))
			// 4. Response
			.Produces<KnowledgeCollectionDto>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
