using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.DeleteQuizAttempt;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Srs.QuizAttempt.DeleteQuizAttempt;

internal sealed class DeleteQuizAttempt : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsAttemptEndpoints()
			.MapDelete("/{attemptId:guid}", async (
				[FromRoute] Guid attemptId,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new DeleteQuizAttemptCommand(attemptId);

				var result = await sender.Send(command, cancellationToken);
				return result.MatchNoContent();
			})
			.WithName("DeleteQuizAttempt")
			.WithSummary("Learner")
			.WithDescription("Cancels an in-progress quiz attempt (soft cancel).")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces(StatusCodes.Status204NoContent)
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
