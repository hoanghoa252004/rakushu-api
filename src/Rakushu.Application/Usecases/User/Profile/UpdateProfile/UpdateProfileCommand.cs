using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.User.Profile.UpdateProfile;

public sealed record UpdateProfileCommand(
	Guid LevelId,
	int DailyLearningMinutes,
	int SessionDurationMinutes,
	IReadOnlyCollection<UpdateInterestDto> Interests,
	string? AvatarKey = null
) : IRequest<Result>;

public sealed record UpdateInterestDto(
	Guid ContentCategoryId,
	int Priority
);
