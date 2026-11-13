using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.Flashcards;
using Rakushu.Application.Usecases.Learning.Srs.Flashcards.GetFlashcardSessionResult;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Srs.Flashcards.GetFlashcardSessionResult;

internal sealed class GetFlashcardSessionResult : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsFlashcardEndpoints()
			.MapGet("/{sessionId:guid}/result", async (
				[FromRoute] Guid sessionId,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var query = new GetFlashcardSessionResultQuery(sessionId);

				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetFlashcardSessionResult")
			.WithSummary("Learner")
			.WithDescription("Retrieves the summary result and review details of a completed flashcard session.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces<FlashcardSessionResultDto>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
