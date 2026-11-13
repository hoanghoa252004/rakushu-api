using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.Flashcards.SubmitFlashcardSession;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Srs.Flashcards.SubmitFlashcardSession;

internal sealed class SubmitFlashcardSession : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsFlashcardEndpoints()
			.MapPatch("/{sessionId:guid}/submit", async (
				[FromRoute] Guid sessionId,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new SubmitFlashcardSessionCommand(sessionId);

				var result = await sender.Send(command, cancellationToken);
				return result.MatchNoContent();
			})
			.WithName("SubmitFlashcardSession")
			.WithSummary("Learner")
			.WithDescription("Submits and finishes an in-progress flashcard session, transitioning its status to Completed.")
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
