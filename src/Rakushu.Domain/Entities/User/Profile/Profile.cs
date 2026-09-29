using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ProficiencyFramework.ProficiencyLevel;
using Rakushu.Domain.SupportedLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

	//NAVIGATION PROPERTIES
	// User
	public User User { get; private set; } = null!;

	// NativeLanguage
	public SupportedLanguage.SupportedLanguage NativeLanguage { get; private set; } = null!;

	// CurrentLevel
	public ProficiencyLevel CurrentLevel { get; private set; } = null!;

	// TargetLevel
	public ProficiencyLevel TargetLevel { get; private set; } = null!;

	// Interests
	private readonly List<Interest.Interest> _interests = new List<Interest.Interest>();
	public IReadOnlyCollection<Interest.Interest> Interests => _interests.AsReadOnly();

	// CONSTRUCTORS & FACTORY METHODS----------
	private Profile() { }

	private Profile(
		ProfileId id,
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
		string fullName,
		SupportedLanguageId? nativeLanguage = null,
		string? avatarKey = null)
	{
		if (string.IsNullOrWhiteSpace(fullName) || fullName.Length > 50)
			return Result.Failure<Profile>(ProfileErrors.InvalidFullName);

		Profile profile = new Profile(
			//fullName,
			//avatarKey,
			//nativeLanguage
			);

		return Result.Success(profile);
	}
}
