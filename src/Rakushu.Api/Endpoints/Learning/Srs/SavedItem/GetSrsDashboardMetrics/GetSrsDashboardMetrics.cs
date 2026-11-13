using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.SavedItem.GetSrsDashboardMetrics;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Srs.SavedItem.GetSrsDashboardMetrics;

internal sealed class GetSrsDashboardMetrics : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsScheduleEndpoints()
			.MapGet("/metrics", async (
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var query = new GetSrsDashboardMetricsQuery();
				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetSrsScheduleMetrics")
			.WithSummary("Learner")
			.WithDescription("Retrieves aggregate learning statistics including due counts, true retention rate (30-day logs), leech count, and mastery rate.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces<SrsDashboardMetricsDto>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
