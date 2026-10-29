using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.DeleteSrsItem;

namespace Rakushu.Api.Endpoints.Learning.Srs.DeleteSrsItem;

internal sealed class DeleteSrsItem : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsItemEndpoints()
			.MapDelete("/items/{id:guid}", async (
				Guid id,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new DeleteSrsItemCommand(id);
				var result = await sender.Send(command, cancellationToken);
				return result.MatchNoContent();
			})
			.WithName("DeleteSrsItem")
			.WithSummary("Remove a saved vocabulary item from personal SRS notebook")
			.WithDescription("Permanently removes the saved item and cascade-deletes its review schedule and history.")
			.RequireAuthorization()
			.Produces(StatusCodes.Status204NoContent)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
