namespace Rakushu.Api.Endpoints.User.Profile;

internal static class ProfileEndpointGroupExtension
{
	internal static RouteGroupBuilder MapProfileEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/profile")
			.WithTags("Profile")
			.WithGroupName("user");
	}
}
