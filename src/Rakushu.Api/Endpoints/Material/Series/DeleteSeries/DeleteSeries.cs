using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Material.Series.DeleteSeries;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Material.Series.DeleteSeries;

internal sealed class DeleteSeries : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSeriesEndpoints()
			.MapDelete("/{id:guid}", async (
			[FromRoute] Guid id,
			ISender sender,
			CancellationToken cancellationToken) =>
		{
			var command = new DeleteSeriesCommand(id);

			var result = await sender.Send(command, cancellationToken);

			return result.MatchOk();
		})
		.WithName("DeleteSeries")
		.WithSummary("Learner & Curator")
		.WithDescription("Deletes a series. Only empty series (with no videos) can be deleted.")
		.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner, RoleCodes.LinguisticCurator))
		.Produces(StatusCodes.Status204NoContent)
		.ProducesProblem(StatusCodes.Status400BadRequest)
		.ProducesProblem(StatusCodes.Status401Unauthorized)
		.ProducesProblem(StatusCodes.Status404NotFound)
		.ProducesProblem(StatusCodes.Status409Conflict)
		.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
