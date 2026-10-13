using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Endpoints.Learning.Video;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Video.CreateVideo;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Api.Endpoints.Learning.Video.CreateVideo;

internal sealed class CreateVideo : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapVideoEndpoints()
			.MapPost("/", async (
				[FromBody] CreateVideoRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new CreateVideoCommand(
					dto.Slug,
					dto.Title,
					dto.Description,
					dto.Duration,
					dto.ContentCategoryId,
					dto.SeriesId,
					dto.SortOrder,
					dto.SourceType,
					dto.SourceUrl,
					dto.StorageKeyOfVideo);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchCreated("GetVideoById", id => new { id });
			})
			.WithName("CreateVideo")
			.WithSummary("Learner, Curator")
			.WithDescription("Creates a new video. Learner and Curator can upload videos. Only Curator can create videos with Curated source.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner, RoleCodes.LinguisticCurator))
			.Produces(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal record CreateVideoRequestDto(
	string Slug,
	string Title,
	string? Description,
	TimeSpan Duration,
	Guid ContentCategoryId,
	Guid? SeriesId,
	int SortOrder,
	VideoSource SourceType,
	string? SourceUrl,
	string StorageKeyOfVideo);
