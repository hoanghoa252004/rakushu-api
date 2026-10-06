using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.JapaneseConjugationForm.Update;

namespace Rakushu.Api.Endpoints.Linguistic.JapaneseConjugationForm.Update;

internal sealed class UpdateJapaneseConjugationFormEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapJapaneseConjugationFormEndpoints()
			.MapPut("/{id}", async (Guid id, [FromBody] UpdateJapaneseConjugationFormRequestDto dto, ISender sender, CancellationToken cancellationToken) =>
			{
				var command = new UpdateJapaneseConjugationFormCommand(id, dto.Name, dto.JapaneseName, dto.Description);
				var result = await sender.Send(command, cancellationToken);
				return result.MatchNoContent();
			})
			.WithName("UpdateJapaneseConjugationForm")
			.WithDescription("Updates an existing Japanese conjugation form.")
			.Produces(StatusCodes.Status204NoContent)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record UpdateJapaneseConjugationFormRequestDto(
	string Name,
	string JapaneseName,
	string? Description);
