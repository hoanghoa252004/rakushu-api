using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Admin.Roles.GetRoles;

namespace Rakushu.Api.Endpoints.Admin.Roles.GetRoles;

internal sealed class GetRoles : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapAdminEndpoints()
			.MapGet("/roles", async (ISender sender, CancellationToken cancellationToken) =>
			{
				var query = new GetRolesQuery();
				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			.WithName("AdminGetRoles")
			.WithDescription("Retrieves the list of available user roles.")
			.Produces<IReadOnlyCollection<RoleDto>>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
