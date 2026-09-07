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
			// 1. Endpoint
			.MapPost("/users", async (
				[FromBody] CreateUserRequestDto dto, 
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new CreateUserCommand(
					dto.Email,
					dto.Password,
					dto.RoleId,
					dto.FullName,
					dto.NativeLanguage,
					dto.AvatarKey
					);

				var result = await sender.Send(command, cancellationToken);
				return result.MatchOk();
			})
			// 2. Description
			.WithName("AdminCreateUser")
			.WithDescription("Creates a new user account directly with specified Role and Status.")
			// 3. Authentication & Authorization: already configure in MapAdminEndpoints()
			// 4. Response
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal record CreateUserRequestDto(
	string Email,
	string Password,
	Guid RoleId,
	string FullName,
	string? NativeLanguage = null,
	string? AvatarKey = null
);