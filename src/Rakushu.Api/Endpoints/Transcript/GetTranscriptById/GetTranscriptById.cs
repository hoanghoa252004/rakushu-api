using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Transcript.GetTranscriptById;

namespace Rakushu.Api.Endpoints.Transcript.GetTranscriptById;

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
