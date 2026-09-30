using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rakushu.Api.Endpoints.Video;

internal static class VideoEndpointGroupExtension
{
	internal static RouteGroupBuilder MapVideoEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/admin/videos")
			.WithTags("Video")
			.WithGroupName("admin")
			.AllowAnonymous();
	}
}
