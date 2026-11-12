using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.SavedItem.GetSrsDashboardMetrics;

namespace Rakushu.Api.Endpoints.Learning.Srs.SavedItem.GetSrsDashboardMetrics;

internal sealed class GetSrsDashboardMetrics : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsItemEndpoints()
			.MapGet("/dashboard/metrics", async (
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var query = new GetSrsDashboardMetricsQuery();
				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetSrsDashboardMetrics")
			.WithSummary("Get SRS dashboard metrics for learner review analysis")
			.WithDescription("Retrieves aggregate learning statistics including due counts, true retention rate (30-day logs), leech count, and mastery rate.")
			.RequireAuthorization()
			.Produces<SrsDashboardMetricsDto>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
