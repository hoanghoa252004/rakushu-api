using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rakushu.Api.Endpoints.Bunsetsu;

internal static class BunsetsuEndpointGroupExtension
{
	internal static RouteGroupBuilder MapBunsetsuEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/admin/bunsetsus")
			.WithTags("Bunsetsu")
			.WithGroupName("admin")
			.AllowAnonymous();
	}
}
