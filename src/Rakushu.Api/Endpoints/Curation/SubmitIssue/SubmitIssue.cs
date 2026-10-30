using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Curation;
using Rakushu.Application.Usecases.Curation.SubmitIssue;

namespace Rakushu.Api.Endpoints.Curation.SubmitIssue;

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
			.WithSummary("Submit an issue report ticket for a learning unit, meaning, or question")
			.WithDescription("Allows learners to report errors, typos, unclear definitions, or audio issues. Aggregates reports and automatically escalates to a Curator ticket when threshold is met.")
			.RequireAuthorization()
			.Produces<SubmitIssueResponse>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record SubmitIssueRequestDto(
	string TargetType,
	Guid TargetId,
	string Category,
	string? LearnerFeedback);
