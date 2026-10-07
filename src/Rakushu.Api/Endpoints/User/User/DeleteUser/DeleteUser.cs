using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.User.User.ChangeUserStatus;
using Rakushu.Application.Usecases.User.User.DeleteUser;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.User.User.DeleteUser;

internal sealed class DeleteUser : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapUserEndpoints()
			// 1. Endpoint
			.MapDelete("/users/{id:guid}", async (
				[FromRoute] Guid id,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var command = new DeleteUserCommand(id);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("DeleteUser")
			.WithSummary("Admin")
			.WithDescription("Permanently deletes a user account and associated profile.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.SystemAdministrator));
	}
}
