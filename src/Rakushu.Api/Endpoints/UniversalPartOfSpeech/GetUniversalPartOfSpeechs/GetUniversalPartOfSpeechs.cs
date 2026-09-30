using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.UniversalPartOfSpeech.GetUniversalPartOfSpeechs;

namespace Rakushu.Api.Endpoints.UniversalPartOfSpeech.GetUniversalPartOfSpeechs;

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
