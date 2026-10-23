using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Material.Transcript;
using Rakushu.Application.Usecases.Material.Transcript.GetLearningTranscript;

namespace Rakushu.Api.Endpoints.Material.Transcript.GetLearningTranscript;

internal sealed class GetLearningTranscript : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapTranscriptEndpoints()
			.MapGet("/learning", async (
			[FromRoute] Guid videoId,
			ISender sender,
			CancellationToken cancellationToken,
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

				var query = new GetLearningTranscriptQuery(videoId, pageNumber, pageSize, startTime);

				var result = await sender.Send(query, cancellationToken);

				return result.MatchOk();
			})
		.WithName("GetLearningTranscript")
		.WithSummary("Allow Anonymous")
		.WithDescription("Retrieves a transcript with its segments for learning purposes.")
		.AllowAnonymous()
		.Produces<PaginatedList<TranscriptSegmentBasicDto>>(StatusCodes.Status200OK)
		.ProducesProblem(StatusCodes.Status404NotFound)
		.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
