using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Authentication.SendPasswordResetCode;

namespace Rakushu.Api.Endpoints.Authentication.SendPasswordResetCode;

internal sealed class SendPasswordResetCode : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapAuthEndpoints()
			// 1. Endpoint
			.MapPost("/password-reset-code", async (
				[FromBody] SendPasswordResetCodeRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var command = new SendPasswordResetCodeCommand(dto.Email);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("SendPasswordResetCode")
			.WithSummary("Allow Anonymous")
			.WithDescription("Send a 6-digit code (valid for 15 minutes) to user email to reset password. A new code can only be requested when the previous one is used or expired.")
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

internal sealed record SendPasswordResetCodeRequestDto(string Email);
