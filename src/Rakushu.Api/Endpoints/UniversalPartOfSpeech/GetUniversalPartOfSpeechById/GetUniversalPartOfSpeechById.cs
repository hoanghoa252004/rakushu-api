using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.UniversalPartOfSpeech.GetUniversalPartOfSpeechById;

namespace Rakushu.Api.Endpoints.UniversalPartOfSpeech.GetUniversalPartOfSpeechById;

internal sealed class GetUniversalPartOfSpeechById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapUniversalPartOfSpeechEndpoints()
			.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetUniversalPartOfSpeechByIdQuery(id), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetUniversalPartOfSpeechById");
	}
}
