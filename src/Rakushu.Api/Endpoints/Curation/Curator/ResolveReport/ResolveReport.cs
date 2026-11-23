using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Curation.Curator.ResolveReport;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Curation.Curator.ResolveReport;

internal sealed class ResolveReport : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapCuratorCurationEndpoints()
			.MapPatch("/{id:guid}/resolve", async (
				[FromRoute] Guid id,
				[FromBody] ResolveReportRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new ResolveReportCommand(id, dto.CuratorNotes);
				var result = await sender.Send(command, cancellationToken);
				return result.MatchNoContent();
			})
			.WithName("ResolveReport")
			.WithSummary("Curator, Administrator")
			.WithDescription("Marks an escalated issue report as resolved with curator notes and resets the category aggregation counter to 0. For ShadowingAttempt reports, a CuratorReevaluation assessment must be submitted before resolving.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator, RoleCodes.SystemAdministrator))
			.Produces(StatusCodes.Status204NoContent)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record ResolveReportRequestDto(string CuratorNotes);
