using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Learning.Video;
using Rakushu.Application.Usecases.Learning.Video.GetCuratedVideos;

namespace Rakushu.Api.Endpoints.Learning.Video.GetCuratedVideos;

internal sealed class GetCuratedVideos : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapVideoEndpoints()
			.MapGet("/curated", async (
				ISender sender,
				[FromQuery] int pageNumber = 1,
				[FromQuery] int pageSize = 10,
				[FromQuery] string? searchKey = null,
				[FromQuery] Guid? contentCategoryId = null,
				[FromQuery] Guid? seriesId = null,
				CancellationToken cancellationToken = default) =>
			{
				var query = new GetCuratedVideosQuery(pageNumber, pageSize, searchKey, contentCategoryId, seriesId);
				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetCuratedVideos")
			.WithSummary("Anonymous")
			.WithDescription("Gets all curated videos with pagination and filtering.")
			.AllowAnonymous()
			.Produces<PaginatedList<VideoDetailDto>>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
