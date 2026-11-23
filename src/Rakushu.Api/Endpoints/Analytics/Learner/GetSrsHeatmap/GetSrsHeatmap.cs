using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Analytics.Learner;
using Rakushu.Application.Usecases.Analytics.Learner.GetSrsHeatmap;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Analytics.Learner.GetSrsHeatmap;

internal sealed class GetSrsHeatmap : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapLearnerAnalyticsEndpoints()
			.MapGet("/srs-heatmap", async (
				[FromQuery] int? year,
				[FromQuery] string? timeZone,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetSrsHeatmapQuery(year, timeZone), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetSrsHeatmap")
			.WithSummary("Learner")
			.WithDescription("Returns daily SRS review heatmap points (date, reviewCount, successCount) for a given calendar year in the requested timezone (default Asia/Ho_Chi_Minh).")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces<SrsHeatmapResponseDto>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
