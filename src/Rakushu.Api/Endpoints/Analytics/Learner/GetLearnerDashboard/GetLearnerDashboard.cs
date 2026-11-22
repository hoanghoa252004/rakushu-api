using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Analytics.Learner;
using Rakushu.Application.Usecases.Analytics.Learner.GetLearnerDashboard;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Analytics.Learner.GetLearnerDashboard;

internal sealed class GetLearnerDashboard : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapLearnerAnalyticsEndpoints()
			.MapGet("/dashboard", async (
				[FromQuery] int? days,
				[FromQuery] int? forecastDays,
				[FromQuery] string? timeZone,
				[FromQuery] int? heatmapYear,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var query = new GetLearnerDashboardQuery(
					days ?? 30,
					forecastDays ?? 14,
					timeZone,
					heatmapYear);
				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetLearnerDashboard")
			.WithSummary("Learner")
			.WithDescription("Returns the learner statistics dashboard summary across F1 (video tracking), F2 (SRS streaks, maturity, heatmap, due forecast), F3 (coverage matrix), F4 (quiz diagnostics), and F5 (shadowing progress).")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces<LearnerDashboardDto>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
