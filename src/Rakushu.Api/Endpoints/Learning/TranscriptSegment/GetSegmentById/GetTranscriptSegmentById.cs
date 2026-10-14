using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Transcript;
using Rakushu.Application.Usecases.Learning.TranscriptSegment.GetSegmentById;

namespace Rakushu.Api.Endpoints.Learning.TranscriptSegment.GetSegmentById;

internal sealed class GetTranscriptSegmentById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapTranscriptSegmentEndpoints()
			.MapGet("/{segmentId:guid}", async (
			[FromRoute] Guid videoId,
			[FromRoute] Guid segmentId,
			ISender sender,
			CancellationToken cancellationToken) =>
		{
			var query = new GetTranscriptSegmentByIdQuery(videoId, segmentId);

			var result = await sender.Send(query, cancellationToken);

			return result.MatchOk();
		})
		.WithTags("TranscriptSegment")
		.WithGroupName("learning")
		.WithName("GetTranscriptSegmentById")
		.WithSummary("Get transcript segment by ID")
		.WithDescription("Retrieves a specific transcript segment with bunsetsus and tokens detail.")
		.Produces<TranscriptSegmentDetailDto>(StatusCodes.Status200OK)
		.ProducesProblem(StatusCodes.Status404NotFound)
		.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
