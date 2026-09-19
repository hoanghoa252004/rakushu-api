using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Users.CreateUser;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Users.CreateUser;

internal sealed class CreateUser : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapUserEndpoints()
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

				return result.MatchCreated("GetUserById", userId => new { id = userId.Value });
			})
			// 2. Description
			.WithName("CreateUser")
			.WithDescription("Creates a new user account directly with specified Role and default status = Inactive.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(DefaultSystemRoles.SystemAdministrator.ToString()))
			// 4. Response
			.Produces(StatusCodes.Status201Created)
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