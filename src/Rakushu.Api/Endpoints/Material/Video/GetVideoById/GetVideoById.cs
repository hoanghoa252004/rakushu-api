using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Material.Video;
using Rakushu.Application.Usecases.Material.Video.GetVideoById;

namespace Rakushu.Api.Endpoints.Material.Video.GetVideoById;

internal sealed class GetVideoById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapVideoEndpoints()
			.MapGet("/{id:guid}", async ([FromRoute] Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetVideoByIdQuery(id), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetVideoById")
			.WithSummary("Anonymous")
			.WithDescription("Gets video by id with all information including content category, series (if available), and media assets.")
			.AllowAnonymous()
			.Produces<VideoDetailDto>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
