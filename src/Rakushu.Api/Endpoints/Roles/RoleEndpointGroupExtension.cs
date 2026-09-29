namespace Rakushu.Api.Endpoints.Roles;

internal static class RoleEndpointGroupExtension
{
	internal static RouteGroupBuilder MapRoleEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/roles")
			.WithTags("Role")
			.WithGroupName("user");
	}
}
