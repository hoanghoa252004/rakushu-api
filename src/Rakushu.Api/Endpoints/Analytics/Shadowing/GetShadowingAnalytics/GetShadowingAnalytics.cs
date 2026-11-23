using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Analytics.Shadowing;
using Rakushu.Application.Usecases.Analytics.Shadowing.GetShadowingAnalytics;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Analytics.Shadowing.GetShadowingAnalytics;

internal sealed class GetShadowingAnalytics : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapShadowingAnalyticsEndpoints()
			.MapGet("", async (
				[FromQuery] string? period,
				[FromQuery] string? timeZone,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetShadowingAnalyticsQuery(period ?? "30d", timeZone), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetShadowingAnalytics")
			.WithSummary("Learner")
			.WithDescription("Returns shadowing KPI metrics (total/completed attempts, total recorded time, completed/skipped line counts, skip rate, average overall score), 3-metric score trend (overall, accuracy, pitch), and assessed attempt history for the requested period ('7d', '30d', '90d', 'all').")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces<ShadowingAnalyticsDetailResponseDto>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
