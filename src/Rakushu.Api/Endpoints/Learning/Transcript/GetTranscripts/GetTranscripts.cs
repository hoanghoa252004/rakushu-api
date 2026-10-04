using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Transcript.GetTranscripts;

namespace Rakushu.Api.Endpoints.Learning.Transcript.GetTranscripts;

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
