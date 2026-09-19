using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Roles.GetRoles;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Roles.GetRoles;

internal sealed class GetRoles : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		// 1. Endpoint
		app.MapGet("/api/roles", async (ISender sender, CancellationToken cancellationToken) =>
			{
				var query = new GetRolesQuery();
				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			// 2. Description
			.WithGroupName("user")
			.WithTags("Role")
			.WithName("AdminGetRoles")
			.WithDescription("Retrieves the list of available user roles.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(DefaultSystemRoles.SystemAdministrator.ToString()))
			// 4. Response
			.Produces<IReadOnlyCollection<GetRolesDto>>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
