using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Users;

internal static class UserEndpointGroupExtension
{
	internal static RouteGroupBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/users")
			.WithGroupName("user")
			.WithTags("User");
	}
}