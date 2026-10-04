using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.JapanesePartOfSpeech.GetJapanesePartOfSpeechs;

namespace Rakushu.Api.Endpoints.Linguistic.JapanesePartOfSpeech.GetJapanesePartOfSpeechs;

internal sealed class GetJapanesePartOfSpeechs : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapJapanesePartOfSpeechEndpoints()
			.MapGet("/", async (ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetJapanesePartOfSpeechsQuery(), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetJapanesePartOfSpeechs");
	}
}
