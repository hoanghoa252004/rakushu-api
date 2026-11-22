using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Analytics.Curator;
using Rakushu.Application.Usecases.Analytics.Curator.GetCuratorDashboard;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Analytics.Curator.GetCuratorDashboard;

internal sealed class GetCuratorDashboard : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapCuratorAnalyticsEndpoints()
			.MapGet("/dashboard", async (
				[FromQuery] int? days,
				[FromQuery] string? timeZone,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var query = new GetCuratorDashboardQuery(
					days ?? 30,
					timeZone);
				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetCuratorDashboard")
			.WithSummary("Curator, Administrator")
			.WithDescription("Returns real-time aggregated curation metrics across reports, videos, question bank, and knowledge catalog for the selected day window.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator, RoleCodes.SystemAdministrator))
			.Produces<CuratorDashboardDto>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
