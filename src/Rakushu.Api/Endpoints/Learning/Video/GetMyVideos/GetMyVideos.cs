using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Learning.Video;
using Rakushu.Application.Usecases.Learning.Video.GetMyVideos;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Video.GetMyVideos;

internal sealed class GetMyVideos : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapVideoEndpoints()
			.MapGet("/me", async (
				ISender sender,
				[FromQuery] int pageNumber = 1,
				[FromQuery] int pageSize = 10,
				[FromQuery] string? searchKey = null,
				[FromQuery] Guid? contentCategoryId = null,
				[FromQuery] Guid? seriesId = null,
				CancellationToken cancellationToken = default) =>
			{
				var query = new GetMyVideosQuery(pageNumber, pageSize, searchKey, contentCategoryId, seriesId);
				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetMyVideos")
			.WithSummary("Learner")
			.WithDescription("Gets videos uploaded by the current learner with pagination and filtering.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces<PaginatedList<VideoListDto>>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
