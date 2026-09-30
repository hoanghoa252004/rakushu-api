using Entity = Rakushu.Domain.Entities.User.Profile.Profile;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User.Profile;

namespace Rakushu.Application.Usecases.ProfileAdmin.UpdateProfile;

public sealed record UpdateProfileCommand(
	Guid ProfileId,
	Guid userId,
	string fullName,
	string? avatarKey,
	Guid nativeLanguageId,
	Guid currentLevelId,
	Guid targetLevelId,
	int dailyLearningMinutes,
	int sessionDurationMinutes
) : IRequest<Result>;
