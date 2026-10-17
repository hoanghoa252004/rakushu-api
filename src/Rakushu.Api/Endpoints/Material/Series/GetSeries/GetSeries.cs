using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Material.Series;
using Rakushu.Application.Usecases.Material.Series.GetSeries;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Material.Series.GetSeries;

internal sealed class GetSeries : IEndpoint
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
			var query = new GetSeriesQuery(pageNumber, pageSize);

			var result = await sender.Send(query, cancellationToken);

			return result.MatchOk();
		})
		.WithName("GetAllSeries")
		.WithSummary("Learner & Curator")
		.WithDescription("Retrieves all series with pagination support.")
		.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner, RoleCodes.LinguisticCurator))
		.Produces<PaginatedList<SeriesDto>>(StatusCodes.Status200OK)
		.ProducesProblem(StatusCodes.Status400BadRequest)
		.ProducesProblem(StatusCodes.Status401Unauthorized)
		.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
