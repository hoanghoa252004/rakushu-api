using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Material.TranscriptSegment;
using Rakushu.Application.Usecases.Material.TranscriptSegment.CreateSegment;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Material.Transcript.CreateTranscriptSegment;

internal sealed class CreateTranscriptSegment : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapTranscriptEndpoints()
			.MapPost("/segments", async (
				[FromRoute] Guid videoId,
				[FromBody] CreateSegmentRequest request,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var command = new CreateTranscriptSegmentCommand(
					videoId,
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
			.WithName("CreateTranscriptSegment")
			.WithSummary("Curator")
			.WithDescription("Creates a new transcript segment with speaker, subtitle and transcript items. Automatically reorders sequences if needed.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			.Produces(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

public sealed record CreateSegmentRequest(
	Guid? SpeakerId,
	string Text,
	TimeSpan StartTime,
	TimeSpan EndTime,
	SubtitleInput? Subtitle,
	List<TranscriptItemInput>? Items,
	int? Sequence = null);
