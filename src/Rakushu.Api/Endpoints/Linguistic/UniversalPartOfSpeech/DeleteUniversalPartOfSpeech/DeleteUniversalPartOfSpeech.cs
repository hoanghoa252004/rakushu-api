using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.UniversalPartOfSpeech.DeleteUniversalPartOfSpeech;

namespace Rakushu.Api.Endpoints.Linguistic.UniversalPartOfSpeech.DeleteUniversalPartOfSpeech;

internal sealed class DeleteUniversalPartOfSpeech : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapUniversalPartOfSpeechEndpoints()
			.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new DeleteUniversalPartOfSpeechCommand(id), cancellationToken);
				return result.MatchOk();
			})
			.WithName("DeleteUniversalPartOfSpeech");
	}
}
