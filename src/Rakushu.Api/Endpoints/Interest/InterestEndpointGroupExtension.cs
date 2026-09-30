using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rakushu.Api.Endpoints.Interest;

internal static class InterestEndpointGroupExtension
{
	internal static RouteGroupBuilder MapInterestEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/admin/interests")
			.WithTags("Interest")
			.WithGroupName("admin")
			.AllowAnonymous();
	}
}
