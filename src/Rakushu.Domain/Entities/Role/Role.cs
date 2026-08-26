using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities.Role;

public sealed class Role : Entity<Guid>
{
	public string RoleName { get; private set; } = null!;
	public string? Description { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }

	// Navigations
	private readonly List<User.User> _users = [];
	public IReadOnlyCollection<User.User> Users => _users.AsReadOnly();

	private Role() { }

	public Role(Guid id, string roleName, string? description = null, DateTimeOffset? createdAt = null)
		: base(id)
	{
		RoleName = roleName;
		Description = description;
		CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
	}

	public static Role Create(string roleName, string? description = null)
	{
		return new Role(Guid.NewGuid(), roleName, description);
	}
}
