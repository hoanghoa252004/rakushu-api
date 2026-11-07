using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.SavedItem.DeleteSrsItem;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Srs.SavedItem.DeleteSrsItem;

internal sealed class DeleteSrsItem : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsSavedItemEndpoints()
			.MapDelete("/{id:guid}", async (
				Guid id,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new DeleteSrsItemCommand(id);
				var result = await sender.Send(command, cancellationToken);
				return result.MatchNoContent();
			})
			.WithName("DeleteSrsItem")
			.WithSummary("Learner")
			.WithDescription("Permanently removes the saved item and cascade-deletes its review schedule and history.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces(StatusCodes.Status204NoContent)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
