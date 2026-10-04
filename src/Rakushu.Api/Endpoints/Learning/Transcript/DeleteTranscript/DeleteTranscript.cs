using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Transcript.DeleteTranscript;

namespace Rakushu.Api.Endpoints.Learning.Transcript.DeleteTranscript;

internal sealed class DeleteTranscript : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapTranscriptEndpoints()
			.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new DeleteTranscriptCommand(id), cancellationToken);
				return result.MatchOk();
			})
			.WithName("DeleteTranscript");
	}
}
