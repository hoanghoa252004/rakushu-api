using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.User.Role.UpdateRole;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.User.Roles.UpdateRole;

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
					dto.Name,
					dto.IsActive,
					dto.Description
				);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("UpdateRole")
			.WithSummary("Admin")
			.WithDescription("Updates an existing role description.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.SystemAdministrator))
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
	string Name,
	bool IsActive,
	string? Description = null
);
