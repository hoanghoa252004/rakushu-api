using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Admin.Users.CreateUser;
using Rakushu.Application.Usecases.Admin.Users.GetUserById;

namespace Rakushu.Api.Endpoints.Admin.Users.CreateUser;

internal sealed class CreateUser : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapAdminEndpoints()
			.MapPost("/users", async ([FromBody] AdminCreateUserRequestDto dto, ISender sender, CancellationToken cancellationToken) =>
			{
				var command = new AdminCreateUserCommand(
					dto.Username,
					dto.Email,
					dto.Password,
					dto.RoleId,
					dto.DisplayName,
					dto.Status,
					dto.NativeLanguage,
					dto.LearningLanguage);

				var result = await sender.Send(command, cancellationToken);
				return result.MatchOk();
			})
			.WithName("AdminCreateUser")
			.WithDescription("Creates a new user account directly with specified Role and Status.")
			.Produces<UserDetailDto>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal record AdminCreateUserRequestDto(
	string Username,
	string Email,
	string Password,
	Guid RoleId,
	string? DisplayName = null,
	string? Status = null,
	string? NativeLanguage = null,
	string? LearningLanguage = null
);
