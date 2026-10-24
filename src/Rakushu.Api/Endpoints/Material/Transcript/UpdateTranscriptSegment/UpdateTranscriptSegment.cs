using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Material.Transcript;
using Rakushu.Application.Usecases.Material.TranscriptSegment.UpdateSegment;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Material.Transcript.UpdateTranscriptSegment;

internal sealed class UpdateTranscriptSegment : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapTranscriptEndpoints()
			.MapPut("/segments/{segmentId:guid}", async (
				[FromRoute] Guid videoId,
				[FromRoute] Guid segmentId,
				[FromBody] UpdateSegmentRequest request,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var command = new UpdateTranscriptSegmentCommand(
					videoId,
					segmentId,
					request.SpeakerId,
					request.Text,
					request.StartTime,
					request.EndTime,
					request.Subtitle,
					request.Items ?? [],
					request.Sequence);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchOk();
			})
			.WithName("UpdateTranscriptSegment")
			.WithSummary("Curator")
			.WithDescription("Updates an existing transcript segment. Automatically reorders sequences if segment position changes.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			.Produces<TranscriptSegmentDetailDto>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

public sealed record UpdateSegmentRequest(
	Guid SpeakerId,
	string Text,
	TimeSpan StartTime,
	TimeSpan EndTime,
	Rakushu.Application.Usecases.Material.TranscriptSegment.SubtitleInput Subtitle,
	List<Rakushu.Application.Usecases.Material.TranscriptSegment.TranscriptItemInput>? Items,
	int? Sequence = null);
