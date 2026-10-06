using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.JapanesePartOfSpeech.Delete;

namespace Rakushu.Api.Endpoints.Linguistic.JapanesePartOfSpeech.Delete;

internal sealed class DeleteJapanesePartOfSpeechEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapJapanesePartOfSpeechEndpoints()
			.MapDelete("/{id}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new DeleteJapanesePartOfSpeechCommand(id), cancellationToken);
				return result.MatchNoContent();
			})
			.WithName("DeleteJapanesePartOfSpeech")
			.WithDescription("Deletes an existing Japanese part of speech.")
			.Produces(StatusCodes.Status204NoContent)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
