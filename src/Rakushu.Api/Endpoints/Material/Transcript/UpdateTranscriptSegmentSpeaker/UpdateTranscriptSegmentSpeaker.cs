using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Material.TranscriptSegment.UpdateSegmentSpeaker;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Material.Transcript.UpdateTranscriptSegmentSpeaker;

internal sealed class UpdateTranscriptSegmentSpeaker : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapTranscriptSegmentEndpoints()
			.MapPatch("/segments/{segmentId:guid}/speaker", async (
				[FromRoute] Guid videoId,
				[FromRoute] Guid segmentId,
				[FromBody] UpdateSegmentSpeakerRequest request,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var command = new UpdateTranscriptSegmentSpeakerCommand(
					videoId,
					segmentId,
					request.SpeakerId,
					request.TranscriptSpeakerId);

				var result = await sender.Send(command, cancellationToken);
				return result.MatchNoContent();
			})
			.WithName("UpdateTranscriptSegmentSpeaker")
			.WithSummary("Curator, Admin")
			.WithDescription("Reassigns or clears the speaker on a single transcript segment without modifying text, subtitle, items, or triggering NLP re-processing.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator, RoleCodes.SystemAdministrator))
			.Produces(StatusCodes.Status204NoContent)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

public sealed record UpdateSegmentSpeakerRequest(
	Guid? SpeakerId = null,
	Guid? TranscriptSpeakerId = null);
