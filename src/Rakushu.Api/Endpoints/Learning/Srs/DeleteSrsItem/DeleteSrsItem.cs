using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.DeleteSrsItem;

namespace Rakushu.Api.Endpoints.Learning.Srs.DeleteSrsItem;

internal sealed class DeleteSrsItem : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsEndpoints()
			// 1. Endpoint
			.MapDelete("/items/{id:guid}", async (
				Guid id,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new DeleteSrsItemCommand(id);
				var result = await sender.Send(command, cancellationToken);
				return result.MatchNoContent();
			})
			// 2. Description
			.WithName("DeleteSrsItem")
			.WithSummary("Remove an SRS item from the user's learning list")
			.WithDescription("Permanently removes the item if it has no quiz history, or safely marks it as Ignored if it has historic quiz attempt references.")
			// 3. Authentication & Authorization
			.RequireAuthorization()
			// 4. Response
			.Produces(StatusCodes.Status204NoContent)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
