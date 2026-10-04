using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.JapanesePartOfSpeech.GetJapanesePartOfSpeechById;

namespace Rakushu.Api.Endpoints.Linguistic.JapanesePartOfSpeech.GetJapanesePartOfSpeechById;

internal sealed class GetJapanesePartOfSpeechById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapJapanesePartOfSpeechEndpoints()
			.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetJapanesePartOfSpeechByIdQuery(id), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetJapanesePartOfSpeechById");
	}
}
