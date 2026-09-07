using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Auth.Login;

namespace Rakushu.Api.Endpoints.Authentication.Login;

internal sealed class Login : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapAuthEndpoints()
			// 1. Endpoint
			.MapPost("/login", async (
				[FromBody] LoginRequestDto dto, 
				ISender sender, 
				CancellationToken cancellationToken
				) =>
			{
				var command = new LoginCommand(dto.Email, dto.Password);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("Login")
			.WithDescription("Authenticates user with Email and Password, returns JWT Access Token & Refresh Token with their expiration")
			// 3. Authentication & Authorization
			.AllowAnonymous()
			// 4. Response
			.Produces<CredentialResponseDto>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record LoginRequestDto(
	string Email,
	string Password
);
