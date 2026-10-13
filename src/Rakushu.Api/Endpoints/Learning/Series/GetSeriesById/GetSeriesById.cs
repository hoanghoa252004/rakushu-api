using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Endpoints.Learning.TranscriptSegment;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Series;
using Rakushu.Application.Usecases.Learning.Series.GetSeriesById;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Series.GetSeriesById;

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

			if (result.IsFailure)
			{
				return Results.NotFound(new { error = result.Error.Message });
			}

			return Results.Ok(result.Value);
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
