using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.TranscriptSegment.DeleteTranscriptSegment;

namespace Rakushu.Api.Endpoints.TranscriptSegment.DeleteTranscriptSegment;

internal sealed class DeleteTranscriptSegment : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapTranscriptSegmentEndpoints()
			.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new DeleteTranscriptSegmentCommand(id), cancellationToken);
				return result.MatchOk();
			})
			.WithName("DeleteTranscriptSegment");
	}
}
