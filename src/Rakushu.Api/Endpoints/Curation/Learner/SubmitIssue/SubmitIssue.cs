using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Curation;
using Rakushu.Application.Usecases.Curation.Learner.SubmitIssue;

namespace Rakushu.Api.Endpoints.Curation.Learner.SubmitIssue;

internal sealed class SubmitIssue : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapLearnerCurationEndpoints()
			.MapPost("", async (
				[FromBody] SubmitIssueRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new SubmitIssueCommand(
					dto.TargetType,
					dto.TargetId,
					dto.Category,
					dto.LearnerFeedback);

				var result = await sender.Send(command, cancellationToken);
				return result.MatchOk();
			})
			.WithName("SubmitLearnerIssue")
			.WithSummary("Learner")
			.WithDescription("Submits an issue report regarding a transcript segment, learning unit, or bank question. Automatically escalates to a curator ticket when the threshold is reached.")
			.RequireAuthorization()
			.Produces<SubmitIssueResponse>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record SubmitIssueRequestDto(
	string TargetType,
	Guid TargetId,
	string Category,
	string? LearnerFeedback);
