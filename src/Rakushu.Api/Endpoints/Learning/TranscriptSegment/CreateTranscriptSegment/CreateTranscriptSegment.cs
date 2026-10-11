using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Endpoints.Learning.Series;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.TranscriptSegment.CreateSegment;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Learning.TranscriptSegment.CreateTranscriptSegment;

internal sealed class CreateTranscriptSegment : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapTranscriptSegmentEndpoints()
			.MapPost("/", async (
				Guid videoId,
				CreateSegmentRequest request,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var command = new CreateTranscriptSegmentCommand(
					videoId,
					request.Text,
					request.StartTime,
					request.EndTime,
					request.Sequence);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchOk();
			})
			.WithName("CreateTranscriptSegment")
			.WithSummary("Create transcript segment")
			.WithDescription("Creates a new transcript segment for the specified video. Automatically reorders sequences if needed.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			.Produces(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

public sealed record CreateSegmentRequest(
	string Text,
	TimeSpan StartTime,
	TimeSpan EndTime,
	int? Sequence = null);
