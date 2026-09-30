using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rakushu.Api.Endpoints.ProfileAdmin;

internal static class ProfileEndpointGroupExtension
{
	internal static RouteGroupBuilder MapProfileEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/admin/profiles")
			.WithTags("Profile")
			.WithGroupName("admin")
			.AllowAnonymous();
	}
}
