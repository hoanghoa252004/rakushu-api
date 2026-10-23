using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Knowledge.KnowledgeMeaning.GetKnowledgeMeaningById;
using Rakushu.Application.Usecases.Knowledge.KnowledgeMeaning.GetKnowledgeMeanings;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Knowledge.KnowledgeMeaning.GetKnowledgeMeanings;

internal sealed class GetKnowledgeMeanings : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapKnowledgeMeaningEndpoints()
			// 1. Endpoint
			.MapGet("/", async (
				Guid knowledgeUnitId,
				ISender sender,
				CancellationToken cancellationToken,
				string? searchTerm = null
				) =>
			{
				var query = new GetKnowledgeMeaningsQuery(knowledgeUnitId, searchTerm);

				var result = await sender.Send(query, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("GetKnowledgeMeanings")
			.WithSummary("Learner (active units only), Curator")
			.WithDescription("Retrieves meanings of a knowledge unit, optionally filtered by searchTerm.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner, RoleCodes.LinguisticCurator))
			// 4. Response
			.Produces<IReadOnlyList<KnowledgeMeaningDto>>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
