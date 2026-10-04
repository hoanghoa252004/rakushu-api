using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Endpoints.Learning.TranscriptSegment;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.TranscriptSegment.UpdateTranscriptSegment;

namespace Rakushu.Api.Endpoints.Learning.TranscriptSegment.UpdateTranscriptSegment;

internal sealed class UpdateTranscriptSegment : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapTranscriptSegmentEndpoints()
			.MapPut("/{id:guid}", async (Guid id, [FromBody] UpdateTranscriptSegmentCommand command, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(command with { TranscriptSegmentId = id }, cancellationToken);
				return result.MatchOk();
			})
			.WithName("UpdateTranscriptSegment");
	}
}
