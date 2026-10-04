using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.UniversalPartOfSpeech.GetUniversalPartOfSpeechById;

namespace Rakushu.Api.Endpoints.Linguistic.UniversalPartOfSpeech.GetUniversalPartOfSpeechById;

internal sealed class GetUniversalPartOfSpeechById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapUniversalPartOfSpeechEndpoints()
			.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetUniversalPartOfSpeechByIdQuery(id), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetUniversalPartOfSpeechById");
	}
}
