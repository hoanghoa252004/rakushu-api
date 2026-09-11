using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities.User.RefreshToken;

public sealed class RefreshToken : Entity<RefreshTokenId>
{
	// MAIN PROPERTIES----------
	public UserId UserId { get; private set; } = null!;
	public string TokenHash { get; private set; } = null!;
	public bool IsRevoked { get; private set; } = false; // DEFAULT VALUE = FALSE
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset ExpiresAt { get; private set; }
	public DateTimeOffset? UsedAt { get; private set; }

	// NAVIGATION PROPERTIES----------
	// User:
	public User User { get; private set; } = null!;

	// COMPUTED PROPERTIES----------
	public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;
	public bool IsActive => !IsRevoked && !IsExpired;

	// CONSTRUCTORS & FACTORY METHODS----------
	private RefreshToken() { }

	private RefreshToken(
		RefreshTokenId id,
		UserId userId,
		string tokenHash,
		DateTimeOffset createdAt,
		DateTimeOffset expiresAt
		) : base(id)
	{
		UserId = userId;
		TokenHash = tokenHash;
		CreatedAt = createdAt;
		ExpiresAt = expiresAt;
	}

	public static RefreshToken Create(UserId userId, string tokenHash, DateTimeOffset createdAt, DateTimeOffset expiresAt)
	{
		RefreshTokenId refreshTokenId = RefreshTokenId.Create();	
		return new RefreshToken(refreshTokenId, userId, tokenHash, createdAt, expiresAt);
	}

	// BEHAVIOR METHODS----------
	public void Revoke(DateTimeOffset? usedAt = null)
	{
		IsRevoked = true;

		if (usedAt is not null)
		{
			UsedAt = usedAt;
		}
	}
}
