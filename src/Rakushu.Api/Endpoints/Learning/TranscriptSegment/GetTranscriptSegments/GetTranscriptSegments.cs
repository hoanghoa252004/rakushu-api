using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.TranscriptSegment.GetTranscriptSegments;

namespace Rakushu.Api.Endpoints.Learning.TranscriptSegment.GetTranscriptSegments;

internal sealed class GetTranscriptSegments : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapTranscriptSegmentEndpoints()
			.MapGet("/", async (ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetTranscriptSegmentsQuery(), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetTranscriptSegments");
	}
}
