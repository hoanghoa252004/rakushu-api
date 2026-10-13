using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Series;
using Rakushu.Application.Usecases.Learning.Series.UpdateSeries;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Series.UpdateSeries;

internal sealed class UpdateSeries : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSeriesEndpoints()
			.MapPut("/{id:guid}", async (
			Guid id,
			UpdateSeriesRequest request,
			ISender sender,
			CancellationToken cancellationToken) =>
		{
			var command = new UpdateSeriesCommand(
				id,
				request.Name,
				request.Description,
				request.DisplayOrder,
				request.IsActive);

			var result = await sender.Send(command, cancellationToken);

			if (result.IsFailure)
			{
				return Results.BadRequest(new { error = result.Error.Message });
			}

			return Results.NoContent();
		})
		.WithTags("Series")
		.WithGroupName("learning")
		.WithName("UpdateSeries")
		.WithSummary("Update a series")
		.WithDescription("Updates a series. SourceType cannot be changed.")
		.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner, RoleCodes.LinguisticCurator))
		.Produces(StatusCodes.Status204NoContent)
		.ProducesProblem(StatusCodes.Status400BadRequest)
		.ProducesProblem(StatusCodes.Status401Unauthorized)
		.ProducesProblem(StatusCodes.Status404NotFound)
		.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

public sealed record UpdateSeriesRequest(
	string Name,
	string? Description,
	int DisplayOrder,
	bool IsActive);
