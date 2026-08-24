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
			.MapPost("/logout", async ([FromBody] LogoutRequestDto dto, ISender sender, CancellationToken cancellationToken) =>
			{
				var command = new LogoutCommand(dto.RefreshToken);
				var result = await sender.Send(command, cancellationToken);
				return result.MatchOk();
			})
			.WithName("Logout")
			.WithDescription("Logs out the user and revokes the specified refresh token.")
			.AllowAnonymous()
			.Produces(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal record LogoutRequestDto(string RefreshToken);
