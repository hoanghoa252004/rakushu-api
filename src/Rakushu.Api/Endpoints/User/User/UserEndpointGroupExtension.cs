namespace Rakushu.Api.Endpoints.User.User;

internal static class UserEndpointGroupExtension
{
	internal static RouteGroupBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/users")
			.WithTags("User")
			.WithGroupName("user");
	}
}