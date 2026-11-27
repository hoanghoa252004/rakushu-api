using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities.Identity.User.PasswordResetCode;

public sealed class PasswordResetCode : Entity<PasswordResetCodeId>
{
	public UserId UserId { get; private set; } = null!;

	public string CodeHash { get; private set; } = null!;

	public DateTimeOffset CreatedAt { get; private set; }

	public DateTimeOffset ExpiresAt { get; private set; }

	public bool IsExpired { get; private set; }

	public bool IsUsed { get; private set; }

	public DateTimeOffset? UsedAt { get; private set; }

	public bool IsActive(DateTimeOffset now) => !IsUsed && !IsExpired && now < ExpiresAt;

	private PasswordResetCode() { }

	private PasswordResetCode(
		PasswordResetCodeId id,
		UserId userId,
		string codeHash,
		DateTimeOffset createdAt,
		DateTimeOffset expiresAt) : base(id)
	{
		UserId = userId;
		CodeHash = codeHash;
		CreatedAt = createdAt;
		ExpiresAt = expiresAt;
	}

	public static PasswordResetCode Create(
		UserId userId,
		string codeHash,
		DateTimeOffset createdAt,
		DateTimeOffset expiresAt)
	{
		return new PasswordResetCode(
			PasswordResetCodeId.Create(),
			userId,
			codeHash,
			createdAt,
			expiresAt);
	}

	public void MarkAsUsed(DateTimeOffset usedAt)
	{
		IsUsed = true;
		UsedAt = usedAt;
	}

	public void MarkAsExpired()
	{
		IsExpired = true;
	}
}
