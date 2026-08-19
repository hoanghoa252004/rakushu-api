using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Endpoints.Authentication;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Auth.Login;

namespace Rakushu.Api.Endpoints.Authentication.Login;

internal sealed class Login : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapAuthEndpoints()
			.MapPost("/login", async ([FromBody] LoginRequestDto dto, ISender _sender) =>
			{
				var command = new LoginCommand(dto.Email, dto.Password);

				var result = await _sender.Send(command);

				return result.MatchOk();
			})
			.WithName("Login")
			.WithDescription($"**Demo accounts:**\n\n" +
							$"- Email: `hoathse184053@fpt.edu.vn` \n" +
							$"- Password: `123456`")
			.AllowAnonymous()
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal record LoginRequestDto(
	string Email,
	string Password
);
