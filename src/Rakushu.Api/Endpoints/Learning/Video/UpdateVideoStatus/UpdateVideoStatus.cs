using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Video.UpdateVideoStatus;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Api.Endpoints.Learning.Video.UpdateVideoStatus;

internal sealed class UpdateVideoStatus : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapVideoEndpoints()
			.MapPatch("/{id:guid}/status", async (
				[FromRoute] Guid id,
				[FromBody] UpdateVideoStatusRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new UpdateVideoStatusCommand(id, dto.NewStatus);
				var result = await sender.Send(command, cancellationToken);
				return result.MatchOk();
			})
			.WithName("UpdateVideoStatus")
			.WithSummary("Curator")
			.WithDescription("Updates the status of a video. Only Curator can update video status. Validates status transitions.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal record UpdateVideoStatusRequestDto(VideoStatus NewStatus);
