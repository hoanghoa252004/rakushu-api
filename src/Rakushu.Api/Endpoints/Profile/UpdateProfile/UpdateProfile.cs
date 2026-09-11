using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Profile.UpdateProfile;

namespace Rakushu.Api.Endpoints.Profile.UpdateProfile;

internal sealed class UpdateProfile : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapProfileEndpoints()
			// 1. Endpoint
			.MapPut("/me", async ([FromBody] UpdateProfileRequestDto dto, ISender sender, CancellationToken cancellationToken) =>
			{
				var command = new UpdateProfileCommand(
					dto.FullName,
					dto.NativeLanguage,
					dto.AvatarKey);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("UpdateMyProfile")
			.WithDescription("Updates the currently authenticated user's profile details.")
			// 3. Authentication & Authorization
			.RequireAuthorization()
			// 4. Response
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal record UpdateProfileRequestDto(
	string FullName,
	string NativeLanguage,
	string? AvatarKey = null
);
