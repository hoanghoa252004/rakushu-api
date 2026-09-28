using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rakushu.Api.Endpoints.Curator;

internal static class CuratorEndpointGroupExtension
{
	internal static RouteGroupBuilder MapCuratorEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/curator/oov")
			.WithTags("Curator OOV")
			.WithGroupName("curator");
	}
}
