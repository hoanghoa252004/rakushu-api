using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.User.RefreshToken;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.User.EmailVerificationToken;

public sealed class EmailVerificationToken : Entity<EmailVerificationTokenId>
{
	public UserId UserId { get; private set; } = null!;

	public string CodeHash { get; private set; } = null!;

	public DateTimeOffset CreatedAt { get; private set; }

	public DateTimeOffset ExpiresAt { get; private set; }

	public DateTimeOffset? UsedAt { get; private set; }

	public bool IsUsed => UsedAt.HasValue;
	public bool IsExpired(DateTimeOffset now) => now >= ExpiresAt;

	private EmailVerificationToken() { }

	private EmailVerificationToken(
		EmailVerificationTokenId id,
		UserId userId,
		string codeHash,
		DateTimeOffset createdAt,
		DateTimeOffset expiresAt)
	{
		Id = id;
		UserId = userId;
		CodeHash = codeHash;
		CreatedAt = createdAt;
		ExpiresAt = expiresAt;
	}

	public static EmailVerificationToken Create(
		UserId userId,
		string codeHash,
		DateTimeOffset createdAt,
		DateTimeOffset expiredAt)
	{
		return new EmailVerificationToken(
			EmailVerificationTokenId.Create(),
			userId,
			codeHash,
			createdAt,
			expiredAt);
	}

	public void MarkAsUsed(DateTimeOffset usedAt)
	{
		UsedAt = usedAt;
	}
}
