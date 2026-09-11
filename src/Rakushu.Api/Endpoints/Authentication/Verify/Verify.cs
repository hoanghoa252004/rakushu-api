using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Auth.Verify;

namespace Rakushu.Api.Endpoints.Authentication.Verify;

internal sealed class Verify : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapAuthEndpoints()
			// 1. Endpoint
			.MapPost("/verify", async (
				[FromBody] VerifyDto dto,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var command = new VerifyCommand(dto.Email, dto.Code);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchOk();
			})
			// 2.Description
			.WithName("Verify")
			.WithDescription("Send  6-digit code to verify account.")
			// 3. Authentication & Authorization
			.AllowAnonymous()
			// 4. Response
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record VerifyDto(
	string Email,
	string Code);
