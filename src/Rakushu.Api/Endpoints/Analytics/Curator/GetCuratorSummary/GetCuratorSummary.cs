using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Analytics.Curator;
using Rakushu.Application.Usecases.Analytics.Curator.GetCuratorSummary;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Analytics.Curator.GetCuratorSummary;

internal sealed class GetCuratorSummary : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapCuratorAnalyticsEndpoints()
			.MapGet("/summary", async (
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetCuratorSummaryQuery(), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetCuratorSummary")
			.WithSummary("Curator, Administrator")
			.WithDescription("Returns lightweight database-wide KPI counts across reports, videos, question bank, and knowledge catalog for Curator workspaces and sidebar badges.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator, RoleCodes.SystemAdministrator))
			.Produces<CuratorSummaryDto>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
