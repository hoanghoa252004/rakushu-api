using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.User.Events;

namespace Rakushu.Domain.Entities.User;

public sealed class User : AggregateRoot<Guid>
{
	private readonly List<RefreshToken> _refreshTokens = [];

	public Guid RoleId { get; private set; }
	public string Username { get; private set; } = null!;
	public string Email { get; private set; } = null!;
	public string PasswordHash { get; private set; } = null!;
	public UserStatus Status { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// Navigations
	public Role.Role? Role { get; private set; }
	public Profile? Profile { get; private set; }
	public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

	private User() { }

	public User(
		Guid id,
		Guid roleId,
		string username,
		string email,
		string passwordHash,
		UserStatus status = UserStatus.Active,
		DateTimeOffset? createdAt = null,
		DateTimeOffset? updatedAt = null)
		: base(id)
	{
		RoleId = roleId;
		Username = username;
		Email = email;
		PasswordHash = passwordHash;
		Status = status;
		CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
		UpdatedAt = updatedAt ?? DateTimeOffset.UtcNow;
	}

	public static User Create(
		string username,
		string email,
		string passwordHash,
		Guid roleId,
		UserStatus status = UserStatus.Active)
	{
		return new User(
			Guid.NewGuid(),
			roleId,
			username,
			email,
			passwordHash,
			status);
	}

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
}
