using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.JapaneseConjugationForm.Delete;

namespace Rakushu.Api.Endpoints.Linguistic.JapaneseConjugationForm.Delete;

internal sealed class DeleteJapaneseConjugationFormEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapJapaneseConjugationFormEndpoints()
			.MapDelete("/{id}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new DeleteJapaneseConjugationFormCommand(id), cancellationToken);
				return result.MatchNoContent();
			})
			.WithName("DeleteJapaneseConjugationForm")
			.WithDescription("Deletes an existing Japanese conjugation form.")
			.Produces(StatusCodes.Status204NoContent)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
