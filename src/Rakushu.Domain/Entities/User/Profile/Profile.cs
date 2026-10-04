using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Linguistic.ProficiencyLevel;
using Rakushu.Domain.Entities.SupportedLanguage;

namespace Rakushu.Domain.Entities.User.Profile;

public sealed class Profile : Entity<ProfileId>
{
	// MAIN PROPERTIES
	public UserId UserId { get; private set; } = null!;
	public string FullName { get; private set; } = null!;
	public string? AvatarKey { get; private set; }
	public SupportedLanguageId NativeLanguageId { get; private set; } = null!;
	public ProficiencyLevelId CurrentLevelId { get; private set; } = null!;
	public ProficiencyLevelId TargetLevelId { get; private set; } = null!;
	public int DailyLearningMinutes { get; private set; }
	public int SessionDurationMinutes { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// User
	public User User { get; private set; } = null!;

	// NativeLanguage
	public SupportedLanguage.SupportedLanguage NativeLanguage { get; private set; } = null!;

	// CurrentLevel
	public ProficiencyLevel CurrentLevel { get; private set; } = null!;

	// TargetLevel
	public ProficiencyLevel TargetLevel { get; private set; } = null!;

	// Interests
	private readonly List<Interest.Interest> _interests = new();
	public IReadOnlyCollection<Interest.Interest> Interests => _interests.AsReadOnly();

	// CONSTRUCTORS & FACTORY METHODS----------
	private Profile() { }

	private Profile(
		ProfileId id,
		UserId userId,
		string fullName,
		SupportedLanguageId nativeLanguageId,
		ProficiencyLevelId currentLevelId,
		ProficiencyLevelId targetLevelId,
		int dailyLearningMinutes,
		int sessionDurationMinutes,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt,
		string? avatarKey = null) : base(id)
	{
		UserId = userId;
		FullName = fullName;
		AvatarKey = avatarKey;
		NativeLanguageId = nativeLanguageId;
		CurrentLevelId = currentLevelId;
		TargetLevelId = targetLevelId;
		DailyLearningMinutes = dailyLearningMinutes;
		SessionDurationMinutes = sessionDurationMinutes;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}



	public static Result<Profile> Create(
		UserId userId,
		string fullName,
		SupportedLanguageId nativeLanguageId,
		ProficiencyLevelId currentLevelId,
		ProficiencyLevelId targetLevelId,
		int dailyLearningMinutes,
		int sessionDurationMinutes,
		DateTimeOffset createdAt,
		string? avatarKey = null)
	{
		if (string.IsNullOrWhiteSpace(fullName) || fullName.Length > 50)
			return Result.Failure<Profile>(ProfileErrors.InvalidFullName);

		if (dailyLearningMinutes <= 0
			|| sessionDurationMinutes <= 0
			|| dailyLearningMinutes <= sessionDurationMinutes)
			return Result.Failure<Profile>(ProfileErrors.InvalidLearningSettings);

		

		return Result.Success(new Profile(
			ProfileId.Create(),
			userId,
			fullName,
			nativeLanguageId,
			currentLevelId,
			targetLevelId,
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
		string fullName,
		SupportedLanguageId nativeLanguageId,
		ProficiencyLevelId currentLevelId,
		ProficiencyLevelId targetLevelId,
		int dailyLearningMinutes,
		int sessionDurationMinutes,
		DateTimeOffset updatedAt,
		string? avatarKey = null)
	{
		if (string.IsNullOrWhiteSpace(fullName) || fullName.Length > 50)
			return Result.Failure(ProfileErrors.InvalidFullName);

		if (dailyLearningMinutes <= 0 
			|| sessionDurationMinutes <= 0
			|| dailyLearningMinutes <= sessionDurationMinutes)
			return Result.Failure(ProfileErrors.InvalidLearningSettings);

		FullName = fullName;
		AvatarKey = avatarKey;
		NativeLanguageId = nativeLanguageId;
		CurrentLevelId = currentLevelId;
		TargetLevelId = targetLevelId;
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

	private bool HasValidInterestPriorities()
	{
		var priorities = _interests
			.Select(x => x.Priority)
			.OrderBy(x => x)
			.ToArray();

		return priorities
			.Select((priority, index) => priority == index + 1)
			.All(x => x);
	}
}
