using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Curation;
using Rakushu.Application.Usecases.Curation.Curator.GetOovStats;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Curation.Curator.GetOovStats;

internal sealed class GetOovStats : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapCuratorCurationEndpoints()
			.MapGet("/oov-stats", async (
				[FromQuery] Guid? videoId,
				[FromQuery] string? status,
				[FromQuery] string? search,
				[FromQuery] int? page,
				[FromQuery] int? pageSize,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var query = new GetOovStatsQuery(
					videoId,
					status,
					search,
					page is null or <= 0 ? 1 : page.Value,
					pageSize is null or <= 0 ? 20 : pageSize.Value);

				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetCuratorOovStats")
			.WithSummary("Curator, Administrator")
			.WithDescription("Retrieves OOV report frequency statistics aggregated by segment and exact character index range for Curated videos only, sorted by highest report count to prioritize handling.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator, RoleCodes.SystemAdministrator))
			.Produces<PaginatedList<CuratorOovStatDto>>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
