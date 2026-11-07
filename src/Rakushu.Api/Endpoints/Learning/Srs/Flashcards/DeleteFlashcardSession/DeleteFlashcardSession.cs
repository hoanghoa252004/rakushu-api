using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.Flashcards.DeleteFlashcardSession;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Srs.Flashcards.DeleteFlashcardSession;

internal sealed class DeleteFlashcardSession : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsFlashcardEndpoints()
			.MapDelete("/{sessionId:guid}", async (
				[FromRoute] Guid sessionId,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new DeleteFlashcardSessionCommand(sessionId);

				var result = await sender.Send(command, cancellationToken);
				return result.MatchOk();
			})
			.WithName("DeleteFlashcardSession")
			.WithSummary("Learner")
			.WithDescription("Deletes a flashcard session and its items completely from the database.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
