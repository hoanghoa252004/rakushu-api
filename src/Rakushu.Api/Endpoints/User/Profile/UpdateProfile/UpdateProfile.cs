using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.User.Profile.UpdateProfile;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.User.Profile.UpdateProfile;

internal sealed class UpdateProfile : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapProfileEndpoints()
			// 1. Endpoint
			.MapPut("/", async (
				[FromBody] UpdateProfileRequestDto dto, 
				ISender sender, 
				CancellationToken cancellationToken
				) =>
			{				
				var command = new UpdateProfileCommand(
					dto.LevelId,
					dto.DailyLearningMinutes,
					dto.SessionDurationMinutes,
					dto.Interests,
					dto.AvatarKey
					);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("UpdateProfile")
			.WithSummary("Learner")
			.WithDescription("Updates the currently authenticated user's profile details including personal info, language preferences, proficiency levels, learning goals, and interests.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			// 4. Response
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal record UpdateProfileRequestDto(
	Guid LevelId,
	int DailyLearningMinutes,
	int SessionDurationMinutes,
	IReadOnlyCollection<UpdateInterestDto> Interests,
	string? AvatarKey = null
);