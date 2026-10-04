using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Endpoints.Learning.TranscriptSegment;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.TranscriptSegment.CreateTranscriptSegment;

namespace Rakushu.Api.Endpoints.Learning.TranscriptSegment.CreateTranscriptSegment;

internal sealed class CreateTranscriptSegment : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapTranscriptSegmentEndpoints()
			.MapPost("/", async ([FromBody] CreateTranscriptSegmentCommand command, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(command, cancellationToken);
				return result.MatchCreated("GetTranscriptSegmentById", id => new { id });
			})
			.WithName("CreateTranscriptSegment");
	}
}
