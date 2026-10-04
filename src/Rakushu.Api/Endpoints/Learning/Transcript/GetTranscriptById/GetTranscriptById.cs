using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Transcript.GetTranscriptById;

namespace Rakushu.Api.Endpoints.Learning.Transcript.GetTranscriptById;

internal sealed class GetTranscriptById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapTranscriptEndpoints()
			.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetTranscriptByIdQuery(id), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetTranscriptById");
	}
}
