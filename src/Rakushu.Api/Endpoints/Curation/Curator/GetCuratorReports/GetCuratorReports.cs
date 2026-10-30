using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
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
				[FromQuery] int page,
				[FromQuery] int pageSize,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var query = new GetCuratorReportsQuery(
					status,
					targetType,
					category,
					page <= 0 ? 1 : page,
					pageSize <= 0 ? 20 : pageSize);

				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetCuratorReports")
			.WithSummary("List triage reports for curators with filters")
			.WithDescription("Allows linguistic curators and admins to filter escalated reports by status (Pending, InReview, Resolved, Dismissed), target type, or category.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator, RoleCodes.SystemAdministrator))
			.Produces<IReadOnlyList<CuratorReportSummaryDto>>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
