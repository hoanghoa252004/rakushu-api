using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Endpoints.Linguistic.UniversalPartOfSpeech;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.UniversalPartOfSpeech.CreateUniversalPartOfSpeech;

namespace Rakushu.Api.Endpoints.Linguistic.UniversalPartOfSpeech.CreateUniversalPartOfSpeech;

internal sealed class CreateUniversalPartOfSpeech : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapUniversalPartOfSpeechEndpoints()
			.MapPost("/", async ([FromBody] CreateUniversalPartOfSpeechCommand command, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(command, cancellationToken);
				return result.MatchCreated("GetUniversalPartOfSpeechById", id => new { id });
			})
			.WithName("CreateUniversalPartOfSpeech");
	}
}
