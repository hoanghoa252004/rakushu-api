using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Knowledge.KnowledgeMeaning.GetMeaningMediaContexts;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Knowledge.KnowledgeMeaning.GetMeaningMediaContexts;

internal sealed class GetMeaningMediaContexts : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapKnowledgeMeaningEndpoints()
			.MapGet("/{meaningId:guid}/media-contexts", async (
				[FromRoute] Guid meaningId,
				[FromQuery] int? maxCount,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var query = new GetMeaningMediaContextsQuery(meaningId, maxCount ?? 10);
				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetMeaningMediaContexts")
			.WithSummary("Curator")
			.WithDescription("Retrieves all video segment occurrences (with presigned video URLs, start/end timestamps, original sentence, and cloze sentence) for a specific knowledge meaning.")
			.RequireAuthorization(policy => policy.RequireRole(
				RoleCodes.LinguisticCurator,
				RoleCodes.SystemAdministrator))
			.Produces<IReadOnlyList<MeaningMediaContextDto>>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
