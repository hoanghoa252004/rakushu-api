using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Auth.ChangePassword;

namespace Rakushu.Api.Endpoints.Authentication.ChangePassword;

internal sealed class ChangePassword : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapAuthEndpoints()
			// 1. Endpoint
			.MapPost("/change-password", async (
				[FromBody] ChangePasswordRequestDto dto,
				ISender sender, 
				CancellationToken cancellationToken
				) =>
			{
				var command = new ChangePasswordCommand(dto.CurrentPassword, dto.NewPassword);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("ChangePassword")
			.WithDescription("Changes the password of the authenticated user.")
			// 3. Authentication & Authorization
			.RequireAuthorization()
			// 4. Response
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record ChangePasswordRequestDto(
	string CurrentPassword,
	string NewPassword
);
