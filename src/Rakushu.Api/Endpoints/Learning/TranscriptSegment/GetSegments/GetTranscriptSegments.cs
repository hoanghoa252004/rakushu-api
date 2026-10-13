using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Learning.Transcript;
using Rakushu.Application.Usecases.Learning.TranscriptSegment.GetSegments;

namespace Rakushu.Api.Endpoints.Learning.TranscriptSegment.GetSegments;

internal sealed class GetTranscriptSegments : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapTranscriptSegmentEndpoints()
			.MapGet("/", async (
			ISender sender,
			CancellationToken cancellationToken,
			[FromRoute] Guid videoId,
			[FromQuery] int pageNumber = 1,
			[FromQuery] int pageSize = 20,
			[FromQuery] long? startTimeMilliseconds = null) =>
		{
			// Convert milliseconds to TimeSpan if provided
			TimeSpan? startTime = null;
			if (startTimeMilliseconds.HasValue && startTimeMilliseconds.Value >= 0)
			{
				startTime = TimeSpan.FromMilliseconds(startTimeMilliseconds.Value);
			}

			var query = new GetTranscriptSegmentsQuery(videoId, pageNumber, pageSize, startTime);

			var result = await sender.Send(query, cancellationToken);

			return result.MatchOk();
		})
		.WithTags("TranscriptSegment")
		.WithGroupName("learning")
		.WithName("GetTranscriptSegments")
		.WithSummary("Get paginated transcript segments")
		.WithDescription("Retrieves paginated transcript segments with optional time filtering. Returns segments with bunsetsus and tokens detail.")
		.Produces<PaginatedList<TranscriptSegmentDetailDto>>(StatusCodes.Status200OK)
		.ProducesProblem(StatusCodes.Status404NotFound)
		.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
