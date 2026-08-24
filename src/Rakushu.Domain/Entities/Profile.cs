using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities;

public sealed class Profile : Entity<Guid>
{
	private Profile() : base() { }

	public Profile(
		Guid userId,
		string displayName,
		string? avatarUrl = null,
		string? bio = null,
		string? nativeLanguage = null,
		string? learningLanguage = null,
		DateTimeOffset? createdAt = null,
		DateTimeOffset? updatedAt = null)
		: base(userId)
	{
		DisplayName = displayName;
		AvatarUrl = avatarUrl;
		Bio = bio;
		NativeLanguage = nativeLanguage;
		LearningLanguage = learningLanguage;
		CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
		UpdatedAt = updatedAt ?? DateTimeOffset.UtcNow;
	}

	public string DisplayName { get; private set; } = string.Empty;
	public string? AvatarUrl { get; private set; }
	public string? Bio { get; private set; }
	public string? NativeLanguage { get; private set; }
	public string? LearningLanguage { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// Navigation
	public User User { get; private set; } = null!;

	public static Profile Create(
		Guid userId,
		string displayName,
		string? avatarUrl = null,
		string? bio = null,
		string? nativeLanguage = null,
		string? learningLanguage = null)
	{
		return new Profile(userId, displayName, avatarUrl, bio, nativeLanguage, learningLanguage);
	}

	public void Update(
		string displayName,
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
