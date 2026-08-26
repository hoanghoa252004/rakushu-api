using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities.User;

public sealed class RefreshToken : Entity<Guid>
{
	public Guid UserId { get; private set; }
	public string Token { get; private set; } = null!;
	public DateTimeOffset ExpiresAt { get; private set; }
	public bool IsRevoked { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }

	// Navigation
	public User? User { get; private set; }

	public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;
	public bool IsActive => !IsRevoked && !IsExpired;

	private RefreshToken() { }

	public RefreshToken(
		Guid id,
		Guid userId,
		string token,
		DateTimeOffset expiresAt,
		bool isRevoked = false,
		DateTimeOffset? createdAt = null)
		: base(id)
	{
		UserId = userId;
		Token = token;
		ExpiresAt = expiresAt;
		IsRevoked = isRevoked;
		CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
	}

	public static RefreshToken Create(Guid userId, string token, DateTimeOffset expiresAt)
	{
		return new RefreshToken(Guid.NewGuid(), userId, token, expiresAt);
	}

	public void Revoke()
	{
		IsRevoked = true;
	}
}
