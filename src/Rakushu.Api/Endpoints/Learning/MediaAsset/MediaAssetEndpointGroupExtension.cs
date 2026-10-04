using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rakushu.Api.Endpoints.Learning.MediaAsset;

internal static class MediaAssetEndpointGroupExtension
{
	internal static RouteGroupBuilder MapMediaAssetEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/admin/media-assets")
			.WithTags("MediaAsset")
			.WithGroupName("admin")
			.AllowAnonymous();
	}
}
