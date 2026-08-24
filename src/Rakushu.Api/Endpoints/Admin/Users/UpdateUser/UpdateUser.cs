using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Admin.Users.GetUserById;
using Rakushu.Application.Usecases.Admin.Users.UpdateUser;

namespace Rakushu.Api.Endpoints.Admin.Users.UpdateUser;

internal sealed class UpdateUser : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapAdminEndpoints()
			.MapPut("/users/{id:guid}", async (
				[FromRoute] Guid id,
				[FromBody] AdminUpdateUserRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new AdminUpdateUserCommand(
					id,
					dto.Username,
					dto.Email,
					dto.RoleId,
					dto.Status,
					dto.DisplayName,
					dto.AvatarUrl,
					dto.Bio,
					dto.NativeLanguage,
					dto.LearningLanguage);

				var result = await sender.Send(command, cancellationToken);
				return result.MatchOk();
			})
			.WithName("AdminUpdateUser")
			.WithDescription("Updates user information, role, status, and profile details.")
			.Produces<UserDetailDto>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal record AdminUpdateUserRequestDto(
	string Username,
	string Email,
	Guid RoleId,
	string Status,
	string DisplayName,
	string? AvatarUrl = null,
	string? Bio = null,
	string? NativeLanguage = null,
	string? LearningLanguage = null
);
