using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Endpoints.Learning.TranscriptSegment;
using Rakushu.Api.Extensions;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Learning.Series;
using Rakushu.Application.Usecases.Learning.Series.GetAllSeries;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Series.GetAllSeries;

internal sealed class GetAllSeries : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSeriesEndpoints()
			.MapGet("/", async (
			ISender sender,
			[FromQuery] int pageNumber = 1,
			[FromQuery] int pageSize = 10,
			CancellationToken cancellationToken = default) =>
		{
			var query = new GetAllSeriesQuery(pageNumber, pageSize);
			var result = await sender.Send(query, cancellationToken);

			if (result.IsFailure)
			{
				return Results.BadRequest(new { error = result.Error.Message });
			}

			return Results.Ok(result.Value);
		})
		.WithName("GetAllSeries")
		.WithSummary("Get all series with pagination")
		.WithDescription("Retrieves all series with pagination support.")
		.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner, RoleCodes.LinguisticCurator))
		.Produces<PaginatedList<SeriesDto>>(StatusCodes.Status200OK)
		.ProducesProblem(StatusCodes.Status400BadRequest)
		.ProducesProblem(StatusCodes.Status401Unauthorized)
		.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
