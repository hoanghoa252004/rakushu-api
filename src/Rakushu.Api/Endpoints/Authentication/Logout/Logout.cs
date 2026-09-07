using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Auth.Logout;

namespace Rakushu.Api.Endpoints.Authentication.Logout;

internal sealed class Logout : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapAuthEndpoints()
			// 1. Endpoint
			.MapPost("/logout", async (
				ISender sender, 
				CancellationToken cancellationToken
				) =>
			{
				var command = new LogoutCommand();

				var result = await sender.Send(command, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("Logout")
			.WithDescription("Logs out the user and revokes the refresh token.")
			// 3. Authentication & Authorization
			.RequireAuthorization()
			// 4. Response
			.Produces(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
