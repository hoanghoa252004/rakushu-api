using Entity = Rakushu.Domain.Entities.User.Profile.Profile;

using Rakushu.Domain.Entities.User.Profile;

namespace Rakushu.Application.Usecases.ProfileAdmin;

public sealed record ProfileDto(
	Guid UserId,
	string FullName,
	string? AvatarKey,
	Guid NativeLanguageId,
	Guid CurrentLevelId,
	Guid TargetLevelId,
	int DailyLearningMinutes,
	int SessionDurationMinutes)
{
	public static ProfileDto FromEntity(Entity e) =>
		new(
			e.UserId.Value,
			e.FullName,
			e.AvatarKey,
			e.NativeLanguageId.Value,
			e.CurrentLevelId.Value,
			e.TargetLevelId.Value,
			e.DailyLearningMinutes,
			e.SessionDurationMinutes);
}
