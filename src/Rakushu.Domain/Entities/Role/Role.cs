using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities.Role;

public sealed class Role : AggregateRoot<RoleId>
{
	// MAIN PROPERTIES----------
	public string Code { get; private set; } = null!;
	public string Name { get; private set; } = null!;
	public string? Description { get; private set; }
	public bool IsActive { get; private set; } = true;
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES----------
	// Users:
	private readonly List<User.User> _users = new();
	public IReadOnlyCollection<User.User> Users => _users.AsReadOnly();

	// CONSTRUCTORS & FACTORY METHODS----------
	private Role() { }

	private Role(RoleId id, string code, string name, bool isActive, DateTimeOffset createdAt, DateTimeOffset updatedAt, string? description = null)
		: base(id)
	{
		Code = code;
		Name = name;
		IsActive = isActive;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
		// Optionals:
		Description = description;
	}

	public static Role Create(string code, string name, bool isActive, DateTimeOffset createdAt, DateTimeOffset updatedAt, string? description = null)
	{
		RoleId roleId = RoleId.Create();
		return new Role(roleId, code, name, isActive, createdAt, updatedAt, description);
	}

	public void Update(string name, bool isActive, DateTimeOffset updatedAt, string? description = null)
	{
		Name = name;
		IsActive = isActive;
		Description = description;
		UpdatedAt = updatedAt;
	}
}
