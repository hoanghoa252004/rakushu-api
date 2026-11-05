using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.Flashcards;
using Rakushu.Application.Usecases.Learning.Srs.Flashcards.GetFlashcardSession;

namespace Rakushu.Api.Endpoints.Learning.Srs.Flashcards.GetFlashcardSession;

internal sealed class GetFlashcardSession : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsFlashcardEndpoints()
			.MapGet("/flashcard-sessions/{id:guid}", async (
				[FromRoute] Guid id,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var query = new GetFlashcardSessionQuery(id);

				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetFlashcardSession")
			.WithSummary("Get flashcard session details and items")
			.WithDescription("Retrieves the specified flashcard session with all vocabulary cards, multimedia video context, and bilingual subtitles.")
			.RequireAuthorization()
			.Produces<FlashcardSessionDetailDto>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
