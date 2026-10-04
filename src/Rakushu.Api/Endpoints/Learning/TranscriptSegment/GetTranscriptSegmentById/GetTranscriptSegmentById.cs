using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.TranscriptSegment.GetTranscriptSegmentById;

namespace Rakushu.Api.Endpoints.Learning.TranscriptSegment.GetTranscriptSegmentById;

internal sealed class GetTranscriptSegmentById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapTranscriptSegmentEndpoints()
			.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetTranscriptSegmentByIdQuery(id), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetTranscriptSegmentById");
	}
}
