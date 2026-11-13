using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.DeleteAttemptDetail;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Srs.QuizAttempt.DeleteAttemptDetail;

internal sealed class DeleteAttemptDetail : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsAttemptEndpoints()
			.MapDelete("/{attemptId:guid}/details/{detailId:guid}", async (
				[FromRoute] Guid attemptId,
				[FromRoute] Guid detailId,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new DeleteAttemptDetailCommand(attemptId, detailId);
				var result = await sender.Send(command, cancellationToken);
				return result.MatchNoContent();
			})
			.WithName("DeleteAttemptDetail")
			.WithSummary("Learner")
			.WithDescription("Deletes an answered attempt detail when the learner deselects an answer, marking the question as unanswered.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces(StatusCodes.Status204NoContent)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
