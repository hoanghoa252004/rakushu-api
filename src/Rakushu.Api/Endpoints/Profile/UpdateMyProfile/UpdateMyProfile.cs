using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Profile.GetMyProfile;
using Rakushu.Application.Usecases.Profile.UpdateMyProfile;

namespace Rakushu.Api.Endpoints.Profile.UpdateMyProfile;

internal sealed class UpdateMyProfile : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapProfileEndpoints()
			.MapPut("/me", async ([FromBody] UpdateProfileRequestDto dto, ISender sender, CancellationToken cancellationToken) =>
			{
				var command = new UpdateMyProfileCommand(
					dto.DisplayName,
					dto.AvatarUrl,
					dto.Bio,
					dto.NativeLanguage,
					dto.LearningLanguage);

				var result = await sender.Send(command, cancellationToken);
				return result.MatchOk();
			})
			.WithName("UpdateMyProfile")
			.WithDescription("Updates the currently authenticated user's profile details.")
			.RequireAuthorization()
			.Produces<ProfileResponseDto>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal record UpdateProfileRequestDto(
	string DisplayName,
	string? AvatarUrl = null,
	string? Bio = null,
	string? NativeLanguage = null,
	string? LearningLanguage = null
);
