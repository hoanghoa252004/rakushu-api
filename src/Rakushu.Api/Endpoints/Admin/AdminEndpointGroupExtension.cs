using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Admin;

internal static class AdminEndpointGroupExtension
{
	internal static RouteGroupBuilder MapAdminEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/admin")
			.WithGroupName("admin")
			.RequireAuthorization(policy => policy.RequireRole(DefaultSystemRoles.SystemAdministrator.ToString()));
	}
}
