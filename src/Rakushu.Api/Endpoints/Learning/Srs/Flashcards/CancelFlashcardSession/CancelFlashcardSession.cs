using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.Flashcards.CancelFlashcardSession;

namespace Rakushu.Api.Endpoints.Learning.Srs.Flashcards.CancelFlashcardSession;

internal sealed class CancelFlashcardSession : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsFlashcardEndpoints()
			.MapPost("/flashcard-sessions/{id:guid}/cancel", async (
				[FromRoute] Guid id,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new CancelFlashcardSessionCommand(id);

				var result = await sender.Send(command, cancellationToken);
				return result.MatchOk();
			})
			.WithName("CancelFlashcardSession")
			.WithSummary("Cancel an active flashcard session")
			.WithDescription("Abandons an in-progress flashcard session so the learner can start another one without waiting for timeout.")
			.RequireAuthorization()
			.Produces(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
