using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.Common;
using Rakushu.Application.Usecases.Learning.Srs.SuspendSrsItem;

namespace Rakushu.Api.Endpoints.Learning.Srs.SuspendSrsItem;

internal sealed class SuspendSrsItem : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsEndpoints()
			// 1. Endpoint
			.MapPatch("/items/{id:guid}/suspend", async (
				Guid id,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new SuspendSrsItemCommand(id);
				var result = await sender.Send(command, cancellationToken);
				return result.MatchOk();
			})
			// 2. Description
			.WithName("SuspendSrsItem")
			.WithSummary("Temporarily suspend an active SRS item")
			.WithDescription("Transitions an active SRS item into Suspended status, freezing its SM-2 progress and excluding it from quizzes.")
			// 3. Authentication & Authorization
			.RequireAuthorization()
			// 4. Response
			.Produces<SrsItemDto>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
