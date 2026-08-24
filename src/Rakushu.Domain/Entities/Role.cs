using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities;

public sealed class Role : Entity<Guid>
{
	private Role() : base() { }

	public Role(Guid id, string roleName, string? description = null, DateTimeOffset? createdAt = null)
		: base(id)
	{
		RoleName = roleName;
		Description = description;
		CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
	}

	public string RoleName { get; private set; } = string.Empty;
	public string? Description { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }

	// Navigation
	public ICollection<User> Users { get; private set; } = new List<User>();

	public static Role Create(string roleName, string? description = null, Guid? id = null)
	{
		return new Role(id ?? Guid.NewGuid(), roleName, description);
	}

	public void Update(string roleName, string? description)
	{
		RoleName = roleName;
		Description = description;
	}
}
