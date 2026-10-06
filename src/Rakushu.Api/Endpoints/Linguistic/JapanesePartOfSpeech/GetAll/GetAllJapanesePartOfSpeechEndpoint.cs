using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Application.Usecases.Linguistic.JapanesePartOfSpeech.GetAll;

namespace Rakushu.Api.Endpoints.Linguistic.JapanesePartOfSpeech.GetAll;

internal sealed class GetAllJapanesePartOfSpeechEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapJapanesePartOfSpeechEndpoints()
			.MapGet("/", async (ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetAllJapanesePartOfSpeechQuery(), cancellationToken);
				return Results.Ok(result);
			})
			.WithName("GetAllJapanesePartOfSpeech");
	}
}
