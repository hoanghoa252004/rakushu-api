using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Auth.Login;
using Rakushu.Application.Usecases.Auth.RefreshToken;

namespace Rakushu.Api.Endpoints.Authentication.RefreshToken;

internal sealed class RefreshToken : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapAuthEndpoints()
			// 1. Endpoint
			.MapPost("/refresh-token", async (
				[FromBody] RefreshTokenRequestDto dto, 
				ISender sender, 
				CancellationToken cancellationToken
				) =>
			{
				var command = new RefreshTokenCommand(dto.RefreshToken);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("RefreshToken")
			.WithDescription("Issues a new Access Token and rotates Refresh Token.")
			// 3. Authentication & Authorization
			.AllowAnonymous()
			// 4. Response
			.Produces<CredentialResponseDto>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record RefreshTokenRequestDto(string RefreshToken);
