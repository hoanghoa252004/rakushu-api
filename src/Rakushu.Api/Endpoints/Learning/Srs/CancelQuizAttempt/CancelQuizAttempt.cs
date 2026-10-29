using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.CancelQuizAttempt;

namespace Rakushu.Api.Endpoints.Learning.Srs.CancelQuizAttempt;

internal sealed class CancelQuizAttempt : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsQuizEndpoints()
			.MapPost("/quiz-attempts/{id:guid}/cancel", async (
				Guid id,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new CancelQuizAttemptCommand(id);
				var result = await sender.Send(command, cancellationToken);
				return result.MatchOk();
			})
			.WithName("CancelSrsQuizAttempt")
			.WithSummary("Cancel an in-progress quiz attempt")
			.WithDescription("Abandons an in-progress quiz attempt without updating SRS schedules or awarding scores, freeing the learner to start a new quiz attempt.")
			.RequireAuthorization()
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
