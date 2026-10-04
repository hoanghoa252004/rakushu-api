using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Endpoints.Linguistic.JapanesePartOfSpeech;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.JapanesePartOfSpeech.UpdateJapanesePartOfSpeech;

namespace Rakushu.Api.Endpoints.Linguistic.JapanesePartOfSpeech.UpdateJapanesePartOfSpeech;

internal sealed class UpdateJapanesePartOfSpeech : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapJapanesePartOfSpeechEndpoints()
			.MapPut("/{id:guid}", async (Guid id, [FromBody] UpdateJapanesePartOfSpeechCommand command, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(command with { JapanesePartOfSpeechId = id }, cancellationToken);
				return result.MatchOk();
			})
			.WithName("UpdateJapanesePartOfSpeech");
	}
}
