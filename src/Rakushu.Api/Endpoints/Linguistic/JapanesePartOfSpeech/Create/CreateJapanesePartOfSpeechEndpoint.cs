using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.JapanesePartOfSpeech.Create;

namespace Rakushu.Api.Endpoints.Linguistic.JapanesePartOfSpeech.Create;

internal sealed class CreateJapanesePartOfSpeechEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapJapanesePartOfSpeechEndpoints()
			.MapPost("/", async (
				[FromBody] CreateJapanesePartOfSpeechRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new CreateJapanesePartOfSpeechCommand(
					dto.Code,
					dto.Name,
					dto.JapaneseName,
					dto.Description);

				var result = await sender.Send(command, cancellationToken);
				return result.MatchCreated("GetJapanesePartOfSpeechById", id => new { id });
			})
			.WithName("CreateJapanesePartOfSpeech")
			.WithDescription("Creates a new Japanese part of speech with Vietnamese and Japanese names.")
			.Produces(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record CreateJapanesePartOfSpeechRequestDto(
	string Code,
	string Name,
	string JapaneseName,
	string? Description = null);
