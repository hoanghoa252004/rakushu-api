using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.User.Role.GetRoles;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.User.Roles.GetRoles;

internal sealed class GetRoles : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapRoleEndpoints()
			// 1. Endpoint
			.MapGet("", async (ISender sender, CancellationToken cancellationToken) =>
			{
				var query = new GetRolesQuery();
				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			// 2. Description
			.WithName("GetRoles")
			.WithDescription("Retrieves the list of available user roles.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.SystemAdministrator))
			// 4. Response
			.Produces<IReadOnlyCollection<GetRolesDto>>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
