using Rakushu.Domain.Common;
using Rakushu.Domain.Enums;

namespace Rakushu.Domain.Entities;

public sealed class User : AggregateRoot<Guid>
{
	private User() : base() { }

	public User(
		Guid id,
		Guid roleId,
		string username,
		string email,
		string passwordHash,
		string status,
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

	public Guid RoleId { get; private set; }
	public string Username { get; private set; } = string.Empty;
	public string Email { get; private set; } = string.Empty;
	public string PasswordHash { get; private set; } = string.Empty;
	public string Status { get; private set; } = UserStatus.Active.ToString();
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// Navigation
	public Role Role { get; private set; } = null!;
	public Profile Profile { get; private set; } = null!;

	public static User Create(
		string username,
		string email,
		string passwordHash,
		Guid roleId,
		string? status = null,
		Guid? id = null)
	{
		var userId = id ?? Guid.NewGuid();
		return new User(
			userId,
			roleId,
			username,
			email,
			passwordHash,
			status ?? UserStatus.Active.ToString()
		);
	}

	public void UpdatePassword(string newPasswordHash)
	{
		PasswordHash = newPasswordHash;
		UpdatedAt = DateTimeOffset.UtcNow;
	}

	public void UpdateStatus(string status)
	{
		Status = status;
		UpdatedAt = DateTimeOffset.UtcNow;
	}

	public void UpdateRole(Guid roleId)
	{
		RoleId = roleId;
		UpdatedAt = DateTimeOffset.UtcNow;
	}

	public void UpdateAdmin(string username, string email, Guid roleId, string status)
	{
		Username = username;
		Email = email;
		RoleId = roleId;
		Status = status;
		UpdatedAt = DateTimeOffset.UtcNow;
	}

	public void SetProfile(Profile profile)
	{
		Profile = profile;
	}
}
