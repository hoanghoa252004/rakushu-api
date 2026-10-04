using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Role;
using Rakushu.Domain.Entities.User.DomainEvents;
using Rakushu.Domain.Entities.User.Profile;
using Rakushu.Domain.Entities.User.ValueObjects.Email;
using System.Net.NetworkInformation;

namespace Rakushu.Domain.Entities.User;

public sealed class User : AggregateRoot<UserId>
{
	// MAIN PROPERTIES----------
	public Email Email { get; private set; } = null!;
	public string PasswordHash { get; private set; } = null!;
	public RoleId RoleId { get; private set; } = null!; // REF: USER * - 1 ROLE
	public UserStatus Status { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES----------
	// Role:
	public Role.Role Role { get; private set; } = null!;

	// RefreshTokens:
	private readonly List<RefreshToken.RefreshToken> _refreshTokens = [];
	public IReadOnlyCollection<RefreshToken.RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

	// EmailVerificationTokens:
	private readonly List<EmailVerificationToken.EmailVerificationToken> _emailVerificationTokens = [];
	public IReadOnlyCollection<EmailVerificationToken.EmailVerificationToken> EmailVerificationTokens => _emailVerificationTokens.AsReadOnly();

	// Subscriptions:
	private readonly List<Subscription.Subscription> _subscriptions = [];
	public IReadOnlyCollection<Subscription.Subscription> Subscriptions => _subscriptions.AsReadOnly();

	// Payments:
	private readonly List<Payment.Payment> _payments = [];
	public IReadOnlyCollection<Payment.Payment> Payments => _payments.AsReadOnly();

	// Profile:
	public Profile.Profile Profile { get; private set; } = null!;

	// Videos:
	private readonly List<Video.Video> _videos = [];
	public IReadOnlyCollection<Video.Video> Videos => _videos.AsReadOnly();

	// CONSTRUCTORS & FACTORY METHODS----------
	private User() { }

	private User(
		UserId id,
		Email email,
		string passwordHash,
		RoleId roleId,
		UserStatus status,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt) : base(id)
	{
		Email = email;
		PasswordHash = passwordHash;
		RoleId = roleId;
		Status = status;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}

	public static Result<User> Create(
		Email email,
		string passwordHash,
		RoleId roleId,
		UserStatus status,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt
		)
	{
		UserId userId = UserId.Create();

		return Result.Success(new User(
					userId,
					email,
					passwordHash,
					roleId,
					status,
					createdAt,
					updatedAt
					));
	}

	public RefreshToken.RefreshToken AddRefreshToken(UserId userId, string hashedToken, DateTimeOffset createdAt, DateTimeOffset expiresAt)
	{
		var refreshToken = RefreshToken.RefreshToken.Create(userId, hashedToken, createdAt, expiresAt);

		_refreshTokens.Add(refreshToken);

		return refreshToken;
	}

	public void RevokeAllActiveRefreshTokens()
	{
		foreach (var refreshToken in _refreshTokens.Where(x => !x.IsRevoked))
		{
			refreshToken.Revoke();
		}
	}

	public void AddEmailVerificationToken(EmailVerificationToken.EmailVerificationToken token)
	{
		_emailVerificationTokens.Add(token);
	}

	public void ChangePassword(string newPasswordHash)
	{
		PasswordHash = newPasswordHash;

		UpdatedAt = DateTimeOffset.UtcNow;

		AddDomainEvent(new UserPasswordChangedDomainEvent(this));
	}

	public void SetProfile(Profile.Profile profile)
	{
		Profile = profile;
		UpdatedAt = DateTimeOffset.UtcNow;
	}

	public Result ChangeStatus(UserStatus status, DateTimeOffset changedAt)
	{
		if(!UserStatusTransition.IsAllowed(Status, status))
		{
			return Result.Failure(CommonErrors.InvalidStatusTransition);
		}

		Status = status;

		UpdatedAt = changedAt;

		if (Status == UserStatus.Banned)
		{
			AddDomainEvent(new UserBannedDomainEvent(this));
		}

		return Result.Success();
	}

	public void VerifyEmail()
	{
		Status = UserStatus.Active;
	}

	public Result IsActive()
	{
		return Status == UserStatus.Active
			? Result.Success()
			: Result.Failure(UserErrors.NotActive);
	}	

	public Result Deactivate(DateTimeOffset deactivatedAt)
	{
		ChangeStatus(UserStatus.Inactive, deactivatedAt);
		RevokeAllActiveRefreshTokens();
		return Result.Success();
	}
}
