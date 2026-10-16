using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Material.TranscriptSegment.DeleteSegment;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Material.TranscriptSegment.DeleteTranscriptSegment;

internal sealed class DeleteTranscriptSegment : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapTranscriptSegmentEndpoints()
			.MapDelete("/{segmentId:guid}", async (
				[FromRoute] Guid videoId,
				[FromRoute] Guid segmentId,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var command = new DeleteTranscriptSegmentCommand(videoId, segmentId);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchOk();
			})
			.WithName("DeleteTranscriptSegment")
			.WithSummary("Delete transcript segment")
			.WithDescription("Deletes a transcript segment and automatically reorders remaining segments.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			.Produces(StatusCodes.Status204NoContent)
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
