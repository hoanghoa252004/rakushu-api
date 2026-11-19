using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Curation.Curator.MarkReportInReview;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Curation.Curator.MarkReportInReview;

internal sealed class MarkReportInReview : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapCuratorCurationEndpoints()
			.MapPatch("/{id:guid}/in-review", async (
				[FromRoute] Guid id,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new MarkReportInReviewCommand(id);
				var result = await sender.Send(command, cancellationToken);
				return result.MatchNoContent();
			})
			.WithName("MarkReportInReview")
			.WithSummary("Curator, Administrator")
			.WithDescription("Transitions a pending report status to InReview so the curator can claim, inspect, and edit the target learning content.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator, RoleCodes.SystemAdministrator))
			.Produces(StatusCodes.Status204NoContent)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
