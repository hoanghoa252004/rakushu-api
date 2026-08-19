namespace Rakushu.Api.Endpoints.Authentication;

internal static class AuthEndpointGroupExtension
{
	internal static RouteGroupBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/auth")
			.WithGroupName("auth")
			.WithTags("Authentication");
	}
}
