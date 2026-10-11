using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Learning.Video;
using Rakushu.Application.Usecases.Learning.Video.GetAllVideos;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Learning.Video.GetAllVideos;

internal sealed class GetAllVideos : IEndpoint
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
				CancellationToken cancellationToken = default) =>
			{
				var query = new GetAllVideosQuery(pageNumber, pageSize, searchKey, contentCategoryId, seriesId, userId);
				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetAllVideos")
			.WithSummary("Curator, Admin")
			.WithDescription("Gets all videos in the system with pagination and filtering. Only Curator and Admin can access.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator, RoleCodes.SystemAdministrator))
			.Produces<PaginatedList<VideoListDto>>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
