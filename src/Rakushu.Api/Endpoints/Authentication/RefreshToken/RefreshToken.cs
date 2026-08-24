using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Auth.RefreshToken;

namespace Rakushu.Api.Endpoints.Authentication.RefreshToken;

internal sealed class RefreshToken : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapAuthEndpoints()
			.MapPost("/refresh-token", async ([FromBody] RefreshTokenRequestDto dto, ISender sender, CancellationToken cancellationToken) =>
			{
				var command = new RefreshTokenCommand(dto.RefreshToken);
				var result = await sender.Send(command, cancellationToken);
				return result.MatchOk();
			})
			.WithName("RefreshToken")
			.WithDescription("Issues a new Access Token and rotates Refresh Token.")
			.AllowAnonymous()
			.Produces<RefreshTokenResponseDto>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal record RefreshTokenRequestDto(string RefreshToken);
