using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Series.DeleteSeries;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Series.DeleteSeries;

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

			if (result.IsFailure)
			{
				return Results.BadRequest(new { error = result.Error.Message });
			}

			return Results.NoContent();
		})
		.WithName("DeleteSeries")
		.WithSummary("Delete a series")
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
