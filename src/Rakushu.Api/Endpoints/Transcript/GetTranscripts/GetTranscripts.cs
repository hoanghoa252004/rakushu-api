using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Transcript.GetTranscripts;

namespace Rakushu.Api.Endpoints.Transcript.GetTranscripts;

internal sealed class GetTranscripts : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapTranscriptEndpoints()
			.MapGet("/", async (ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetTranscriptsQuery(), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetTranscripts");
	}
}
