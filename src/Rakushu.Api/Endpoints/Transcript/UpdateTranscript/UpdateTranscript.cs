using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Transcript.UpdateTranscript;

namespace Rakushu.Api.Endpoints.Transcript.UpdateTranscript;

internal sealed class UpdateTranscript : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapTranscriptEndpoints()
			.MapPut("/{id:guid}", async (Guid id, [FromBody] UpdateTranscriptCommand command, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(command with { TranscriptId = id }, cancellationToken);
				return result.MatchOk();
			})
			.WithName("UpdateTranscript");
	}
}
