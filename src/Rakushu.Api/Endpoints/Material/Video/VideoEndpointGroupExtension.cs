using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rakushu.Api.Endpoints.Material.Video;

internal static class VideoEndpointGroupExtension
{
	internal static RouteGroupBuilder MapVideoEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/videos")
			.WithTags("Video")
			.WithGroupName("material");
	}
}
