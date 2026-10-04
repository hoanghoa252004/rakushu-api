using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.UniversalPartOfSpeech.GetUniversalPartOfSpeechs;

namespace Rakushu.Api.Endpoints.Linguistic.UniversalPartOfSpeech.GetUniversalPartOfSpeechs;

internal sealed class GetUniversalPartOfSpeechs : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapUniversalPartOfSpeechEndpoints()
			.MapGet("/", async (ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetUniversalPartOfSpeechsQuery(), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetUniversalPartOfSpeechs");
	}
}
