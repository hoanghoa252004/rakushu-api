using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Role;
using Rakushu.Domain.Entities.User.DomainEvents;
using Rakushu.Domain.Entities.User.EmailVerificationToken;
using Rakushu.Domain.Entities.User.ValueObjects.Email;
using Rakushu.Domain.Entities.User.ValueObjects.Profile;

namespace Rakushu.Domain.Entities.User;

public sealed class User : AggregateRoot<UserId>
{
	// MAIN PROPERTIES----------
	public Email Email { get; private set; } = null!;
	public string PasswordHash { get; private set; } = null!;
	public RoleId RoleId { get; private set; } = null!; // REF: USER * - 1 ROLE
	public UserStatus Status { get; private set; }
	public Profile Profile { get; private set; } = null!;
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

	// CONSTRUCTORS & FACTORY METHODS----------
	private User() { }

	private User(
		UserId id,
		Email email,
		string passwordHash,
		RoleId roleId,
		UserStatus status,
		Profile profile,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt) : base(id)
	{
		Email = email;
		PasswordHash = passwordHash;
		RoleId = roleId;
		Status = status;
		Profile = profile;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}

	public static Result<User> Create(
		Email email,
		string passwordHash,
		RoleId roleId,
		UserStatus status,
		Profile profile,
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
					profile,
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

	public void UpdateProfile(Profile profile)
	{
		Profile = profile;
	}

	public Result ChangeStatus(UserStatus status)
	{
		if(!UserStatusTransition.IsAllowed(Status, status))
		{
			return Result.Failure(CommonError.InvalidStatusTransition);
		}

		Status = status;

		if(Status == UserStatus.Banned)
		{
			AddDomainEvent(new UserBannedDomainEvent(this));
		}

		return Result.Success();
	}

	public EmailVerificationToken.EmailVerificationToken AddEmailVerificationToken(UserId userId, string hashedCode, DateTimeOffset createdAt, DateTimeOffset expiresAt)
	{
		var emailVerificationToken = EmailVerificationToken.EmailVerificationToken.Create(userId, hashedCode, createdAt, expiresAt);

		_emailVerificationTokens.Add(emailVerificationToken);

		return emailVerificationToken;
	}
	/*
	public void SetProfile(Profile profile)
	{
		Profile = profile;
	}

	public void UpdatePassword(string newPasswordHash)
	{
		PasswordHash = newPasswordHash;
		UpdatedAt = DateTimeOffset.UtcNow;
		AddDomainEvent(new UserPasswordChangedDomainEvent(Id));
	}

	public void ChangeStatus(UserStatus newStatus)
	{
		Status = newStatus;
		UpdatedAt = DateTimeOffset.UtcNow;
	}

	public void UpdateAccount(string username, string email, Guid roleId, UserStatus status)
	{
		Username = username;
		Email = email;
		RoleId = roleId;
		Status = status;
		UpdatedAt = DateTimeOffset.UtcNow;
	}

	public RefreshToken AddRefreshToken(string token, DateTimeOffset expiresAt)
	{
		var refreshToken = RefreshToken.Create(Id, token, expiresAt);
		_refreshTokens.Add(refreshToken);
		return refreshToken;
	}

	public void RevokeRefreshToken(string token)
	{
		var existing = _refreshTokens.FirstOrDefault(t => t.Token == token);
		existing?.Revoke();
	}

	public void RevokeAllRefreshTokens()
	{
		foreach (var token in _refreshTokens.Where(t => !t.IsRevoked))
		{
			token.Revoke();
		}
	}
	*/
}
