using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.Flashcards;
using Rakushu.Application.Usecases.Learning.Srs.Flashcards.GetFlashcardSession;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Srs.Flashcards.GetFlashcardSession;

internal sealed class GetFlashcardSession : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsFlashcardEndpoints()
			.MapGet("/{sessionId:guid}", async (
				[FromRoute] Guid sessionId,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var query = new GetFlashcardSessionQuery(sessionId);

				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetFlashcardSession")
			.WithSummary("Learner")
			.WithDescription("Retrieves the specified flashcard session with all vocabulary cards, multimedia video context, and review status.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces<FlashcardSessionDetailDto>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
