using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Authentication.ResetPassword;

namespace Rakushu.Api.Endpoints.Authentication.ResetPassword;

internal sealed class ResetPassword : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapAuthEndpoints()
			// 1. Endpoint
			.MapPost("/reset-password", async (
				[FromQuery] string code,
				[FromBody] ResetPasswordRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var command = new ResetPasswordCommand(dto.Email, code, dto.NewPassword);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("ResetPassword")
			.WithSummary("Allow Anonymous")
			.WithDescription("Reset password with the 6-digit code (query string) sent to user email.")
			// 3. Authentication & Authorization
			.AllowAnonymous()
			// 4. Response
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record ResetPasswordRequestDto(
	string Email,
	string NewPassword);
