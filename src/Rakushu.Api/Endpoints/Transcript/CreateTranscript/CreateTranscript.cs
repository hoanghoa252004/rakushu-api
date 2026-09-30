using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Transcript.CreateTranscript;

namespace Rakushu.Api.Endpoints.Transcript.CreateTranscript;

internal sealed class CreateTranscript : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapTranscriptEndpoints()
			.MapPost("/", async ([FromBody] CreateTranscriptCommand command, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(command, cancellationToken);
				return result.MatchCreated("GetTranscriptById", id => new { id });
			})
			.WithName("CreateTranscript");
	}
}
