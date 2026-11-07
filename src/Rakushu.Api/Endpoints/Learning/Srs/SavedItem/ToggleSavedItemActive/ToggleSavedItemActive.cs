using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.SavedItem.ToggleSavedItemActive;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Srs.SavedItem.ToggleSavedItemActive;

internal sealed class ToggleSavedItemActive : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsSavedItemEndpoints()
			.MapPut("/{id:guid}", async (
				Guid id,
				[FromBody] ToggleSavedItemActiveRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new ToggleSavedItemActiveCommand(id, dto.IsActive);
				var result = await sender.Send(command, cancellationToken);
				return result.MatchNoContent();
			})
			.WithName("ToggleSavedItemActive")
			.WithSummary("Learner")
			.WithDescription("Enables or disables learning and SRS review for this vocabulary item.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces(StatusCodes.Status204NoContent)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record ToggleSavedItemActiveRequestDto(bool IsActive);
