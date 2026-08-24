using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities;

public sealed class RefreshToken : Entity<Guid>
{
	private RefreshToken() : base() { }

	public RefreshToken(Guid id, Guid userId, string token, DateTimeOffset expiresAt, DateTimeOffset? createdAt = null)
		: base(id)
	{
		UserId = userId;
		Token = token;
		ExpiresAt = expiresAt;
		CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
		IsRevoked = false;
	}

	public Guid UserId { get; private set; }
	public string Token { get; private set; } = string.Empty;
	public DateTimeOffset ExpiresAt { get; private set; }
	public bool IsRevoked { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }

	// Navigation
	public User User { get; private set; } = null!;

	public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;
	public bool IsActive => !IsRevoked && !IsExpired;

	public static RefreshToken Create(Guid userId, string token, DateTimeOffset expiresAt)
	{
		return new RefreshToken(Guid.NewGuid(), userId, token, expiresAt);
	}

	public void Revoke()
	{
		IsRevoked = true;
	}
}
