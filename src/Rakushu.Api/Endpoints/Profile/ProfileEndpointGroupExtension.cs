namespace Rakushu.Api.Endpoints.Profile;

internal static class ProfileEndpointGroupExtension
{
	internal static RouteGroupBuilder MapProfileEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/profile")
			.WithGroupName("auth")
			.WithTags("Profile");
	}
}
