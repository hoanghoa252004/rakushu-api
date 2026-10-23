using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Material.Series;
using Rakushu.Application.Usecases.Material.Series.GetSeriesById;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Material.Series.GetSeriesById;

internal sealed class GetSeriesById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSeriesEndpoints()
			.MapGet("/{id:guid}", async (
			[FromRoute] Guid id,
			ISender sender,
			CancellationToken cancellationToken) =>
		{
			var query = new GetSeriesByIdQuery(id);

			var result = await sender.Send(query, cancellationToken);

			return result.MatchOk();
		})
		.WithName("GetSeriesById")
		.WithSummary("Get series by ID")
		.WithDescription("Retrieves a single series by its ID.")
		.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner, RoleCodes.LinguisticCurator))
		.Produces<SeriesDto>(StatusCodes.Status200OK)
		.ProducesProblem(StatusCodes.Status401Unauthorized)
		.ProducesProblem(StatusCodes.Status404NotFound)
		.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
