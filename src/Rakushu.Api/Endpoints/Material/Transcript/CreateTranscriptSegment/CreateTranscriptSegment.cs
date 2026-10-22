using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Material.TranscriptSegment.CreateSegment;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Material.Transcript.CreateTranscriptSegment;

internal sealed class CreateTranscriptSegment : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapTranscriptEndpoints()
			.MapPost("/segments", async (
				Guid videoId,
				CreateSegmentRequest request,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var command = new CreateTranscriptSegmentCommand(
					request.SpeakerId,
					videoId,
					request.Text,
					request.StartTime,
					request.EndTime,
					request.Sequence);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchOk();
			})
			.WithName("CreateTranscriptSegment")
			.WithSummary("Curator")
			.WithDescription("Creates a new transcript segment for the specified video. Automatically reorders sequences if needed.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			.Produces(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

public sealed record CreateSegmentRequest(
	Guid SpeakerId,
	string Text,
	TimeSpan StartTime,
	TimeSpan EndTime,
	int? Sequence = null);
