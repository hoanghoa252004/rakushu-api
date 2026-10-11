using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Transcript;
using Rakushu.Application.Usecases.Learning.Transcript.GetTranscript;

namespace Rakushu.Api.Endpoints.Learning.Transcript.GetTranscript;

internal sealed class GetTranscript : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapTranscriptEndpoints()
			.MapGet("/", async (
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

			var query = new GetTranscriptQuery(videoId, pageNumber, pageSize, startTime);

			var result = await sender.Send(query, cancellationToken);

			return result.MatchOk();
		})
		.WithTags("Transcript")
		.WithGroupName("learning")
		.WithName("GetTranscript")
		.WithSummary("Get transcript with segments")
		.WithDescription("Retrieves a transcript and its segmented content with pagination and optional time filtering. Returns transcript data and paginated segments with bunsetsus and tokens detail.")
		.Produces<TranscriptDetailDto>(StatusCodes.Status200OK)
		.ProducesProblem(StatusCodes.Status404NotFound)
		.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
