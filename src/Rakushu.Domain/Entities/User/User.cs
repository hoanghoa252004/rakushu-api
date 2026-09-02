using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Domain.Entities.User;

public sealed class User : AggregateRoot<UserId>
{
	// MAIN PROPERTIES----------
	public string Email { get; private set; } = null!;
	public string PasswordHash { get; private set; } = null!;
	public string FullName { get; private set; } = null!;
	public string? AvatarKey { get; private set; }
	public string NativeLanguage { get; private set; } = null!;
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

	// CONSTRUCTORS & FACTORY METHODS----------
	private User() { }

	private User(
		UserId id,
		string email,
		string passwordHash,
		string fullName,
		string nativeLanguage,
		RoleId roleId,
		UserStatus status,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt,
		string? avatarKey = null) : base(id)
	{
		Email = email;
		PasswordHash = passwordHash;
		FullName = fullName;
		NativeLanguage = nativeLanguage;
		RoleId = roleId;
		Status = status;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
		// Optionals:
		AvatarKey = avatarKey;
	}

	public static User Create(
		string email,
		string passwordHash,
		string fullName,
		string nativeLanguage,
		RoleId roleId,
		UserStatus status,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt,
		string? avatarKey = null)
	{
		UserId userId = UserId.Create();
		return new User(
					userId,
					email,
					passwordHash,
					fullName,
					nativeLanguage,
					roleId,
					status,
					createdAt,
					updatedAt,
					avatarKey
		);
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
