using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Material.Video;
using Rakushu.Application.Usecases.Material.Video.GetVideos;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Material.Video.GetVideos;

internal sealed class GetVideos : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapVideoEndpoints()
			.MapGet("/", async (
				ISender sender,
				[FromQuery] int pageNumber = 1,
				[FromQuery] int pageSize = 10,
				[FromQuery] string? searchKey = null,
				[FromQuery] Guid? contentCategoryId = null,
				[FromQuery] Guid? seriesId = null,
				[FromQuery] Guid? userId = null,
				[FromQuery] string? status = null,
				CancellationToken cancellationToken = default) =>
			{
				var query = new GetVideosQuery(pageNumber, pageSize, searchKey, contentCategoryId, seriesId, userId, status);
				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetAllVideos")
			.WithSummary("Curator, Admin")
			.WithDescription("Gets all videos in the system with pagination and filtering. Only Curator and Admin can access.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator, RoleCodes.SystemAdministrator))
			.Produces<PaginatedList<VideoDetailDto>>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
