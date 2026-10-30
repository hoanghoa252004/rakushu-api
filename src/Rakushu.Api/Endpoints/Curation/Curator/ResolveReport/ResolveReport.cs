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
			.MapPost("/{id:guid}/resolve", async (
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
			.WithSummary("Mark report as resolved with curator notes and reset aggregator counter")
			.WithDescription("Resolves the curator report and resets the category issue counter to 0.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator, RoleCodes.SystemAdministrator))
			.Produces(StatusCodes.Status204NoContent)
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record ResolveReportRequestDto(string CuratorNotes);
