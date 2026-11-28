using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Analytics.Admin;
using Rakushu.Application.Usecases.Analytics.Admin.GetAdminMaterialsAnalytics;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Analytics.Admin;

internal sealed class GetAdminMaterialsAnalytics : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapAdminAnalyticsEndpoints()
			.MapGet("/materials", async (
				[FromQuery] int? daysWindow,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(
					new GetAdminMaterialsAnalyticsQuery(daysWindow ?? 7),
					cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetAdminMaterialsAnalytics")
			.WithSummary("Administrator")
			.WithDescription("Returns platform-wide learning materials analytics (KPIs, source breakdown, top 5 categories, cloud storage breakdown by asset type, and video status distribution) aggregated directly in the database. Supports optional daysWindow query parameter (1-365, default 7).")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.SystemAdministrator))
			.Produces<AdminMaterialsAnalyticsDto>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
