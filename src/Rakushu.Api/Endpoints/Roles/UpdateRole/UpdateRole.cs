using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Roles.UpdateRole;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Roles.UpdateRole;

internal sealed class UpdateRole : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapRoleEndpoints()
			// 1. Endpoint
			.MapPut("/{id:guid}", async (
				[FromRoute] Guid id,
				[FromBody] UpdateRoleRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var command = new UpdateRoleCommand(
					id,
					dto.Description,
					dto.IsActive
				);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("UpdateRole")
			.WithDescription("Updates an existing role with the provided description and active status. When deactivating a role, all refresh tokens of users in that role will be revoked.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(DefaultSystemRoles.SystemAdministrator.ToString()))
			// 4. Response
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record UpdateRoleRequestDto(
	string? Description = null,
	bool? IsActive = null
);
