using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.User.Profile.CreateProfile;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.User.Profile.CreateProfile;

internal sealed class CreateProfile : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapProfileEndpoints()
			// 1. Endpoint
			.MapPost("/", async (
				[FromBody] CreateProfileRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var command = new CreateProfileCommand(
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
			.WithName("CreateProfile")
			.WithSummary("Learner")
			.WithDescription("Creates a new profile for the currently authenticated user including personal info, language preferences, proficiency levels, learning goals, and interests.")
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

internal record CreateProfileRequestDto(
	Guid LevelId,
	int DailyLearningMinutes,
	int SessionDurationMinutes,
	IReadOnlyCollection<CreateInterestDto> Interests,
	string? AvatarKey = null
);