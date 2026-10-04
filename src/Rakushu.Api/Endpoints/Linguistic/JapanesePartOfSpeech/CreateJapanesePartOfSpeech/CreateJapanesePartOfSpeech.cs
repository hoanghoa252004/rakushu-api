using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Endpoints.Linguistic.JapanesePartOfSpeech;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.JapanesePartOfSpeech.CreateJapanesePartOfSpeech;

namespace Rakushu.Api.Endpoints.Linguistic.JapanesePartOfSpeech.CreateJapanesePartOfSpeech;

internal sealed class CreateJapanesePartOfSpeech : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapJapanesePartOfSpeechEndpoints()
			.MapPost("/", async ([FromBody] CreateJapanesePartOfSpeechCommand command, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(command, cancellationToken);
				return result.MatchCreated("GetJapanesePartOfSpeechById", id => new { id });
			})
			.WithName("CreateJapanesePartOfSpeech");
	}
}
