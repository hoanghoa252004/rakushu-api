using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Curation.DeleteMyIssue;

namespace Rakushu.Api.Endpoints.Curation.DeleteMyIssue;

internal sealed class DeleteMyIssue : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapLearnerCurationEndpoints()
			.MapDelete("/{id:guid}", async (
				[FromRoute] Guid id,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new DeleteMyIssueCommand(id);
				var result = await sender.Send(command, cancellationToken);
				return result.MatchNoContent();
			})
			.WithName("DeleteMyIssue")
			.WithSummary("Delete or withdraw a pending issue report submitted by current user")
			.WithDescription("Allows learners to cancel/delete their own submitted feedback as long as the report is not already in review or resolved.")
			.RequireAuthorization()
			.Produces(StatusCodes.Status204NoContent)
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
