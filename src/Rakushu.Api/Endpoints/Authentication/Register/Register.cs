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
			.MapPost("/register", async ([FromBody] RegisterRequestDto dto, ISender sender, CancellationToken cancellationToken) =>
			{
				var command = new RegisterCommand(
					dto.Username,
					dto.Email,
					dto.Password,
					dto.DisplayName,
					dto.NativeLanguage,
					dto.LearningLanguage);

				var result = await sender.Send(command, cancellationToken);
				return result.MatchOk();
			})
			.WithName("Register")
			.WithDescription("Registers a new user account with default User role and creates an associated Profile.")
			.AllowAnonymous()
			.Produces<RegisterResponseDto>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal record RegisterRequestDto(
	string Username,
	string Email,
	string Password,
	string? DisplayName = null,
	string? NativeLanguage = null,
	string? LearningLanguage = null
);
