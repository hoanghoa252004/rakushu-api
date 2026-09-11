using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Auth.Register;

namespace Rakushu.Api.Endpoints.Authentication.Register;

internal sealed class Register : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapAuthEndpoints()
			// 1. Endpoint
			.MapPost("/register", async (
				[FromBody] RegisterRequestDto dto, 
				ISender sender, 
				CancellationToken cancellationToken
				) =>
			{
				var command = new RegisterCommand(
					dto.Email,
					dto.Password,
					dto.FullName,
					dto.NativeLanguage,
					dto.AvatarKey
					);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchOk();
			})
			// 2.Description
			.WithName("Register")
			.WithDescription("Registers a new user account with default User role and creates an associated Profile.")
			// 3. Authentication & Authorization
			.AllowAnonymous()
			// 4. Response
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal record RegisterRequestDto(
	string Email,
	string Password,
	string FullName,
	string NativeLanguage,
	string? AvatarKey = null
);
