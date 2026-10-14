using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Series;
using Rakushu.Application.Usecases.Learning.Series.GetSeriesBySlug;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Series.GetSeriesBySlug;

internal sealed class GetSeriesBySlug : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSeriesEndpoints()
			.MapGet("/{slug:alpha}", async (
			[FromQuery]string slug,
			ISender sender,
			CancellationToken cancellationToken) =>
		{
			var query = new GetSeriesBySlugQuery(slug);
			var result = await sender.Send(query, cancellationToken);

			if (result.IsFailure)
			{
				return Results.NotFound(new { error = result.Error.Message });
			}

			return Results.Ok(result.Value);
		})
		.WithName("GetSeriesBySlug")
		.WithSummary("Get series by slug")
		.WithDescription("Retrieves a single series by its slug.")
		.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner, RoleCodes.LinguisticCurator))
		.Produces<SeriesDto>(StatusCodes.Status200OK)
		.ProducesProblem(StatusCodes.Status401Unauthorized)
		.ProducesProblem(StatusCodes.Status404NotFound)
		.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
