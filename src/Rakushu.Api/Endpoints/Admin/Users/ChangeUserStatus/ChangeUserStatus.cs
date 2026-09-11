using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Admin.Users.ChangeUserStatus;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Api.Endpoints.Admin.Users.ChangeUserStatus;

internal sealed class ChangeUserStatus : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapAdminEndpoints()
			// 1. Endpoint
			.MapPatch("/users/{id:guid}/status", async (
				[FromRoute] Guid id,
				[FromBody] ChangeUserStatusRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var command = new ChangeUserStatusCommand(id, dto.Status);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithTags("User")
			.WithName("AdminChangeUserStatus")
			.WithDescription("Updates account status (Active, Inactive, Banned) of a user.")
			// 3. Authentication & Authorization: already configure in MapAdminEndpoints()
			// 4. Response
			.Produces(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal record ChangeUserStatusRequestDto(UserStatus Status);
