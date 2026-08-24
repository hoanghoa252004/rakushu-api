using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Admin.Users.ChangeStatus;

namespace Rakushu.Api.Endpoints.Admin.Users.ChangeStatus;

internal sealed class ChangeUserStatus : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapAdminEndpoints()
			.MapPatch("/users/{id:guid}/status", async (
				[FromRoute] Guid id,
				[FromBody] ChangeUserStatusRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new AdminChangeUserStatusCommand(id, dto.Status);
				var result = await sender.Send(command, cancellationToken);
				return result.MatchOk();
			})
			.WithName("AdminChangeUserStatus")
			.WithDescription("Updates account status (Active, Inactive, Banned) of a user.")
			.Produces(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal record ChangeUserStatusRequestDto(string Status);
