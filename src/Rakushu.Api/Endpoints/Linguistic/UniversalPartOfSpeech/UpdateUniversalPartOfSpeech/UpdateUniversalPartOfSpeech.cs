using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Endpoints.Linguistic.UniversalPartOfSpeech;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.UniversalPartOfSpeech.UpdateUniversalPartOfSpeech;

namespace Rakushu.Api.Endpoints.Linguistic.UniversalPartOfSpeech.UpdateUniversalPartOfSpeech;

internal sealed class UpdateUniversalPartOfSpeech : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapUniversalPartOfSpeechEndpoints()
			.MapPut("/{id:guid}", async (Guid id, [FromBody] UpdateUniversalPartOfSpeechCommand command, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(command with { UniversalPartOfSpeechId = id }, cancellationToken);
				return result.MatchOk();
			})
			.WithName("UpdateUniversalPartOfSpeech");
	}
}
