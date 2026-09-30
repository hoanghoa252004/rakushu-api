using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.TranscriptSegment.GetTranscriptSegments;

namespace Rakushu.Api.Endpoints.TranscriptSegment.GetTranscriptSegments;

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
