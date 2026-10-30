using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Curation.Curator.DismissReport;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Curation.Curator.DismissReport;

internal sealed class DismissReport : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapCuratorCurationEndpoints()
			.MapPost("/{id:guid}/dismiss", async (
				[FromRoute] Guid id,
				[FromBody] DismissReportRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new DismissReportCommand(id, dto.Reason);
				var result = await sender.Send(command, cancellationToken);
				return result.MatchNoContent();
			})
			.WithName("DismissReport")
			.WithSummary("Dismiss report as false alarm or not an error")
			.WithDescription("Closes the curator report with an explanation of why no change was needed.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator, RoleCodes.SystemAdministrator))
			.Produces(StatusCodes.Status204NoContent)
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record DismissReportRequestDto(string Reason);
