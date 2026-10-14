using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Series.GetVideoBySeriesSlugAndVideoSlug;
using Rakushu.Application.Usecases.Learning.Video;

namespace Rakushu.Api.Endpoints.Learning.Series.GetVideoBySeriesSlugAndVideoSlug;

internal sealed class GetVideoBySeriesSlugAndVideoSlug : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app
			.MapSeriesEndpoints()
			.MapGet("/{seriesSlug:alpha}/videos/{videoSlug:alpha}", async (
			[FromRoute] string seriesSlug,
			[FromRoute] string videoSlug,
			ISender sender,
			CancellationToken cancellationToken) =>
		{
			var query = new GetVideoBySeriesSlugAndVideoSlugQuery(seriesSlug, videoSlug);
			var result = await sender.Send(query, cancellationToken);

			if (result.IsFailure)
			{
				return Results.BadRequest(new { error = result.Error.Message });
			}

			return Results.Ok(result.Value);
		})
		.WithTags("Series")
		.WithGroupName("learning")
		.WithName("GetVideoBySeriesSlugAndVideoSlug")
		.WithSummary("Get video by series slug and video slug")
		.WithDescription("Retrieves a single video from a series using series slug and video slug.")
		.Produces<VideoDto>(StatusCodes.Status200OK)
		.ProducesProblem(StatusCodes.Status404NotFound)
		.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
