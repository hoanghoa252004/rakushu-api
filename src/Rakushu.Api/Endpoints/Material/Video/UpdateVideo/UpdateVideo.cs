using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Endpoints.Material.Video;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Material.Video.UpdateVideo;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Material.Video.UpdateVideo;

internal sealed class UpdateVideo : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapVideoEndpoints()
			.MapPut("/{id:guid}", async (
				[FromRoute] Guid id,
				[FromBody] UpdateVideoRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new UpdateVideoCommand(
					id,
					dto.Title,
					dto.Description,
					dto.ContentCategoryId,
					dto.SeriesId,
					dto.SortOrder);

				var result = await sender.Send(command, cancellationToken);
				return result.MatchOk();
			})
			.WithName("UpdateVideo")
			.WithSummary("Learner, Curator")
			.WithDescription("Updates basic info of a video (Title, Description, ContentCategoryId, SeriesId, SortOrder). Does not update status.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner, RoleCodes.LinguisticCurator))
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal record UpdateVideoRequestDto(
	string Title,
	string? Description,
	Guid ContentCategoryId,
	Guid? SeriesId,
	int SortOrder);
