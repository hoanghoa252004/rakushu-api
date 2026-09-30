using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.JapanesePartOfSpeech.DeleteJapanesePartOfSpeech;

namespace Rakushu.Api.Endpoints.JapanesePartOfSpeech.DeleteJapanesePartOfSpeech;

internal sealed class DeleteJapanesePartOfSpeech : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapJapanesePartOfSpeechEndpoints()
			.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new DeleteJapanesePartOfSpeechCommand(id), cancellationToken);
				return result.MatchOk();
			})
			.WithName("DeleteJapanesePartOfSpeech");
	}
}
