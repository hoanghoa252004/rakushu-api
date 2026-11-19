using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Curation;
using Rakushu.Application.Usecases.Curation.Curator.GetCuratorReports;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Curation.Curator.GetCuratorReports;

internal sealed class GetCuratorReports : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapCuratorCurationEndpoints()
			.MapGet("", async (
				[FromQuery] string? status,
				[FromQuery] string? targetType,
				[FromQuery] string? category,
				[FromQuery] int? page,
				[FromQuery] int? pageSize,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var query = new GetCuratorReportsQuery(
					status,
					targetType,
					category,
					page is null or <= 0 ? 1 : page.Value,
					pageSize is null or <= 0 ? 20 : pageSize.Value);

				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetCuratorReports")
			.WithSummary("Curator, Administrator")
			.WithDescription("Retrieves a paginated list of escalated reports for curators, filterable by status, target type, and category.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator, RoleCodes.SystemAdministrator))
			.Produces<PaginatedList<CuratorReportSummaryDto>>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
