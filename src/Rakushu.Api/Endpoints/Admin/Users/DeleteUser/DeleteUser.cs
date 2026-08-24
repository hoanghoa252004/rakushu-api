using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Admin.Users.DeleteUser;

namespace Rakushu.Api.Endpoints.Admin.Users.DeleteUser;

internal sealed class DeleteUser : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapAdminEndpoints()
			.MapDelete("/users/{id:guid}", async (
				[FromRoute] Guid id,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new AdminDeleteUserCommand(id);
				var result = await sender.Send(command, cancellationToken);
				return result.MatchOk();
			})
			.WithName("AdminDeleteUser")
			.WithDescription("Permanently deletes a user account and associated profile.")
			.Produces(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
