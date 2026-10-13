using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Endpoints.Learning.Series;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Transcript;
using Rakushu.Application.Usecases.Learning.TranscriptSegment.UpdateSegment;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.TranscriptSegment.UpdateTranscriptSegment;

internal sealed class UpdateTranscriptSegment : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapTranscriptSegmentEndpoints()
			.MapPut("/{segmentId:guid}", async (
				[FromRoute] Guid videoId,
				[FromRoute] Guid segmentId,
				[FromBody] UpdateSegmentRequest request,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var command = new UpdateTranscriptSegmentCommand(
					videoId,
					segmentId,
					request.Text,
					request.StartTime,
					request.EndTime,
					request.Sequence);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchOk();
			})
			.WithName("UpdateTranscriptSegment")
			.WithSummary("Update transcript segment")
			.WithDescription("Updates an existing transcript segment. Automatically reorders sequences if segment position changes.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			.Produces<TranscriptSegmentDetailDto>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

public sealed record UpdateSegmentRequest(
	string Text,
	TimeSpan StartTime,
	TimeSpan EndTime,
	int? Sequence = null);
