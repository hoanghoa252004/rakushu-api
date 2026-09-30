using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rakushu.Api.Endpoints.Token;

internal static class TokenEndpointGroupExtension
{
	internal static RouteGroupBuilder MapTokenEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/admin/tokens")
			.WithTags("Token")
			.WithGroupName("admin")
			.AllowAnonymous();
	}
}
