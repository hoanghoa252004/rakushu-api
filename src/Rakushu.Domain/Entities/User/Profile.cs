using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities.User;

public sealed class Profile : Entity<Guid>
{
	public string? DisplayName { get; private set; }
	public string? AvatarUrl { get; private set; }
	public string? Bio { get; private set; }
	public string? NativeLanguage { get; private set; }
	public string? LearningLanguage { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// Navigation
	public User? User { get; private set; }

	private Profile() { }

	public Profile(
		Guid userId,
		string? displayName = null,
		string? avatarUrl = null,
		string? bio = null,
		string? nativeLanguage = null,
		string? learningLanguage = "Japanese",
		DateTimeOffset? createdAt = null,
		DateTimeOffset? updatedAt = null)
		: base(userId)
	{
		DisplayName = displayName;
		AvatarUrl = avatarUrl;
		Bio = bio;
		NativeLanguage = nativeLanguage;
		LearningLanguage = learningLanguage ?? "Japanese";
		CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
		UpdatedAt = updatedAt ?? DateTimeOffset.UtcNow;
	}

	public static Profile Create(
		Guid userId,
		string? displayName = null,
		string? avatarUrl = null,
		string? bio = null,
		string? nativeLanguage = null,
		string? learningLanguage = "Japanese")
	{
		return new Profile(userId, displayName, avatarUrl, bio, nativeLanguage, learningLanguage);
	}

	public void Update(
		string? displayName,
		string? avatarUrl,
		string? bio,
		string? nativeLanguage,
		string? learningLanguage)
	{
		DisplayName = displayName;
		AvatarUrl = avatarUrl;
		Bio = bio;
		NativeLanguage = nativeLanguage;
		LearningLanguage = learningLanguage;
		UpdatedAt = DateTimeOffset.UtcNow;
	}
}
