using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.Flashcards.UpdateFlashcardSessionItem;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Srs.Flashcards.UpdateFlashcardSessionItem;

internal sealed class UpdateFlashcardSessionItem : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsFlashcardEndpoints()
			.MapPut("/{sessionId:guid}/items/{itemId:guid}", async (
				[FromRoute] Guid sessionId,
				[FromRoute] Guid itemId,
				[FromBody] UpdateFlashcardSessionItemRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new UpdateFlashcardSessionItemCommand(sessionId, itemId, dto.Rating);

				var result = await sender.Send(command, cancellationToken);
				return result.MatchNoContent();
			})
			.WithName("UpdateFlashcardSessionItem")
			.WithSummary("Learner")
			.WithDescription("Updates the rating ('Forgot', 'Hard', 'Easy') for an already reviewed flashcard item before final submission.")
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

internal sealed record UpdateFlashcardSessionItemRequestDto(
	string Rating);

