using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.JapanesePartOfSpeech.GetJapanesePartOfSpeechs;

namespace Rakushu.Api.Endpoints.JapanesePartOfSpeech.GetJapanesePartOfSpeechs;

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
