using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ContentCategory;
using Rakushu.Domain.Entities.ProficiencyFramework.ProficiencyLevel;
using Rakushu.Domain.Entities.User.Profile.Interest;
using Rakushu.Domain.SupportedLanguage;

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
		DateTimeOffset updatedAt,
		string? avatarKey = null)
	{
		if (string.IsNullOrWhiteSpace(fullName) || fullName.Length > 50)
			return Result.Failure<Profile>(ProfileErrors.InvalidFullName);

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
			updatedAt,
			avatarKey));
	}

	public static Result<Profile> Create(
		string fullName,
		SupportedLanguageId? nativeLanguage = null,
		string? avatarKey = null)
	{
		if (string.IsNullOrWhiteSpace(fullName) || fullName.Length > 50)
			return Result.Failure<Profile>(ProfileErrors.InvalidFullName);

		var now = DateTimeOffset.UtcNow;
		return Result.Success(new Profile(
			ProfileId.Create(),
			UserId.Create(),
			fullName,
			nativeLanguage ?? SupportedLanguageId.Create(),
			ProficiencyLevelId.Create(),
			ProficiencyLevelId.Create(),
			0,
			0,
			now,
			now,
			avatarKey));
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

	public Result<Interest.Interest> AddInterest(ContentCategoryId categoryId, int priority)
	{
		var interest = Interest.Interest.Create(Id, categoryId, priority);
		if (interest.IsFailure)
			return interest;

		_interests.Add(interest.Value);
		return interest;
	}

	public Result UpdateInterest(InterestId interestId, int priority)
	{
		var interest = _interests.FirstOrDefault(i => i.Id == interestId);
		if (interest is null)
			return Result.Failure(ProfileErrors.InterestNotFound);

		interest.Update(priority);
		return Result.Success();
	}

	public Result RemoveInterest(InterestId interestId)
	{
		var interest = _interests.FirstOrDefault(i => i.Id == interestId);
		if (interest is null)
			return Result.Failure(ProfileErrors.InterestNotFound);

		_interests.Remove(interest);
		return Result.Success();
	}
}
