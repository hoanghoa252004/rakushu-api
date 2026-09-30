using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Transcript.DeleteTranscript;

namespace Rakushu.Api.Endpoints.Transcript.DeleteTranscript;

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
