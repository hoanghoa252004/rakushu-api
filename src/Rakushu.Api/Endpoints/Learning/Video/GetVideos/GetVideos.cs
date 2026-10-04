using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Video.GetVideos;

namespace Rakushu.Api.Endpoints.Learning.Video.GetVideos;

internal sealed class GetVideos : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapVideoEndpoints()
			.MapGet("/", async (ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetVideosQuery(), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetVideos");
	}
}
