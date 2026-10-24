using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Material.Transcript;
using Rakushu.Application.Usecases.Material.Transcript.GetDetailTranscript;

namespace Rakushu.Api.Endpoints.Material.Transcript.GetDetailTranscript;

internal sealed class GetDetailTranscript : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapTranscriptEndpoints()
			.MapGet("/detail", async (
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

			var query = new GetDetailTranscriptQuery(videoId, pageNumber, pageSize, startTime);

			var result = await sender.Send(query, cancellationToken);

			return result.MatchOk();
		})
		.WithName("GetDetailTranscript")
		.WithSummary("Get transcript detail")
		.WithDescription("Retrieves a transcript detail with paginated segments and optional time filtering for curator ( segment + item + learning unit + bunsetsu + token )")
		.Produces<PaginatedList<TranscriptSegmentDetailDto>>(StatusCodes.Status200OK)
		.ProducesProblem(StatusCodes.Status404NotFound)
		.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
