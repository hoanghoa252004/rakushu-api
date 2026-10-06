using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.JapanesePartOfSpeech.Update;

namespace Rakushu.Api.Endpoints.Linguistic.JapanesePartOfSpeech.Update;

internal sealed class UpdateJapanesePartOfSpeechEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapJapanesePartOfSpeechEndpoints()
			.MapPut("/{id}", async (Guid id, [FromBody] UpdateJapanesePartOfSpeechRequestDto dto, ISender sender, CancellationToken cancellationToken) =>
			{
				var command = new UpdateJapanesePartOfSpeechCommand(id, dto.Name, dto.JapaneseName, dto.Description);
				var result = await sender.Send(command, cancellationToken);
				return result.MatchNoContent();
			})
			.WithName("UpdateJapanesePartOfSpeech")
			.WithDescription("Updates an existing Japanese part of speech.")
			.Produces(StatusCodes.Status204NoContent)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record UpdateJapanesePartOfSpeechRequestDto(
	string Name,
	string JapaneseName,
	string? Description);
