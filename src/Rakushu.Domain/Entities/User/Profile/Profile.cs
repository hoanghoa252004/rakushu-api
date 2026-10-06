using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ProficiencyLevel;

namespace Rakushu.Domain.Entities.User.Profile;

public sealed class Profile : Entity<ProfileId>
{
	// MAIN PROPERTIES
	public UserId UserId { get; private set; } = null!;
	public string? AvatarKey { get; private set; }
	public ProficiencyLevelId LevelId { get; private set; } = null!;
	public int DailyLearningMinutes { get; private set; }
	public int SessionDurationMinutes { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// User
	public User User { get; private set; } = null!;

	// Level
	public ProficiencyLevel.ProficiencyLevel Level { get; private set; } = null!;

	// Interests
	private readonly List<Interest.Interest> _interests = new();
	public IReadOnlyCollection<Interest.Interest> Interests => _interests.AsReadOnly();

	// CONSTRUCTORS & FACTORY METHODS----------
	private Profile() { }

	private Profile(
		ProfileId id,
		UserId userId,
		ProficiencyLevelId levelId,
		int dailyLearningMinutes,
		int sessionDurationMinutes,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt,
		string? avatarKey = null) : base(id)
	{
		UserId = userId;
		AvatarKey = avatarKey;
		LevelId = levelId;
		DailyLearningMinutes = dailyLearningMinutes;
		SessionDurationMinutes = sessionDurationMinutes;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}



	public static Result<Profile> Create(
		UserId userId,
		ProficiencyLevelId levelId,
		int dailyLearningMinutes,
		int sessionDurationMinutes,
		DateTimeOffset createdAt,
		string? avatarKey = null)
	{
		if (dailyLearningMinutes <= 0
			|| sessionDurationMinutes <= 0
			|| dailyLearningMinutes <= sessionDurationMinutes)
			return Result.Failure<Profile>(ProfileErrors.InvalidLearningSettings);

		return Result.Success(new Profile(
			ProfileId.Create(),
			userId,
			levelId,
			dailyLearningMinutes,
			sessionDurationMinutes,
			createdAt,
			createdAt,
			avatarKey
		));
	}

	public Result AddInterests(IReadOnlyCollection<Interest.Interest> inputs)
	{
		if (inputs
		.GroupBy(x => x.ContentCategoryId)
		.Any(x => x.Count() > 1))
			return Result.Failure(
				ProfileErrors.DuplicateInterestCategory);

		if (inputs.GroupBy(x => x.Priority)
					.Any(x => x.Count() > 1))
			return Result.Failure(
				ProfileErrors.DuplicateInterestPriority);

		if (inputs.Count == 0)
			return Result.Failure(
				ProfileErrors.ContainAtLeast1Interest);

		_interests.AddRange(inputs);

		return Result.Success();
	}

	public Result Update(
		ProficiencyLevelId levelId,
		int dailyLearningMinutes,
		int sessionDurationMinutes,
		DateTimeOffset updatedAt,
		string? avatarKey = null)
	{
		if (dailyLearningMinutes <= 0 
			|| sessionDurationMinutes <= 0
			|| dailyLearningMinutes <= sessionDurationMinutes)
			return Result.Failure(ProfileErrors.InvalidLearningSettings);

		AvatarKey = avatarKey;
		LevelId = levelId;
		DailyLearningMinutes = dailyLearningMinutes;
		SessionDurationMinutes = sessionDurationMinutes;
		UpdatedAt = updatedAt;

		return Result.Success();
	}

	public Result UpdateInterests(IReadOnlyCollection<Interest.Interest> inputs)
	{
		if (inputs
		.GroupBy(x => x.ContentCategoryId)
		.Any(x => x.Count() > 1))
			return Result.Failure(
				ProfileErrors.DuplicateInterestCategory);

		if (inputs.GroupBy(x => x.Priority)
					.Any(x => x.Count() > 1))
			return Result.Failure(
				ProfileErrors.DuplicateInterestPriority);

		if(inputs.Count == 0)
			return Result.Failure(
				ProfileErrors.ContainAtLeast1Interest);

		_interests.Clear();

		_interests.AddRange(inputs);

		return Result.Success();
	}
}
