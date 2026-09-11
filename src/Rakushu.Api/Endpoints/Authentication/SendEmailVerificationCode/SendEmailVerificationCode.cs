using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Endpoints.Authentication.Register;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Auth.SendEmailVerificationCode;

namespace Rakushu.Api.Endpoints.Authentication.SendEmailVerificationCode;

internal sealed class SendEmailVerificationCode : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapAuthEndpoints()
			// 1. Endpoint
			.MapPost("/email-verification-code", async (
				[FromBody] SendEmailVerificationCodeRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var command = new SendEmailVerificationCodeCommand(dto.Email);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchOk();
			})
			// 2.Description
			.WithName("SendEmailVerificationCode")
			.WithDescription("Create a hash code then send to user email to confirm.")
			// 3. Authentication & Authorization
			.AllowAnonymous()
			// 4. Response
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record SendEmailVerificationCodeRequestDto(string Email);
