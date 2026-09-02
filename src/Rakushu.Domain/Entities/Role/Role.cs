using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities.Role;

public sealed class Role : AggregateRoot<RoleId>
{
	// MAIN PROPERTIES----------
	public string Title { get; private set; } = null!;
	public string? Description { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES----------
	// Users:
	private readonly List<User.User> _users = [];
	public IReadOnlyCollection<User.User> Users => _users.AsReadOnly();

	// CONSTRUCTORS & FACTORY METHODS----------
	private Role() { }

	private Role(RoleId id, string title, DateTimeOffset createdAt, DateTimeOffset updatedAt, string? description = null)
		: base(id)
	{
		Title = title;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
		// Optionals:
		Description = description;
	}

	public static Role Create(string title, DateTimeOffset createdAt, DateTimeOffset updatedAt, string? description = null)
	{
		RoleId roleId = RoleId.Create();
		return new Role(roleId, title, createdAt, updatedAt, description);
	}
}
