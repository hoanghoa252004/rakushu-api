using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.JapaneseConjugationForm.Create;

namespace Rakushu.Api.Endpoints.Linguistic.JapaneseConjugationForm.Create;

internal sealed class CreateJapaneseConjugationFormEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapJapaneseConjugationFormEndpoints()
			.MapPost("/", async (
				[FromBody] CreateJapaneseConjugationFormRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new CreateJapaneseConjugationFormCommand(
					dto.Code,
					dto.Name,
					dto.JapaneseName,
					dto.Description);

				var result = await sender.Send(command, cancellationToken);
				return result.MatchCreated("GetJapaneseConjugationFormById", id => new { id });
			})
			.WithName("CreateJapaneseConjugationForm")
			.WithDescription("Creates a new Japanese conjugation form with Vietnamese and Japanese names.")
			.Produces(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record CreateJapaneseConjugationFormRequestDto(
	string Code,
	string Name,
	string JapaneseName,
	string? Description = null);
