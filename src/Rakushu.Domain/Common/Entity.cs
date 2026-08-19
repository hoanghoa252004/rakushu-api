namespace Rakushu.Domain.Common;

public abstract class Entity<TKey> : IEquatable<Entity<TKey>>
{
	// Constructors
	protected Entity() { }
	protected Entity(TKey id)
	{
		Id = id;
	}

	// Properties
	public TKey Id { get; protected init; } = default!;

	// Methods
	public bool Equals(Entity<TKey>? other)
	{
		if (other is null) return false;
		if (ReferenceEquals(this, other)) return true;
		return EqualityComparer<TKey>.Default.Equals(Id, other.Id);
	}

	public override bool Equals(object? other)
	{
		return other is Entity<TKey> entity && Equals(entity);
	}

	public override int GetHashCode()
	{
		return Id!.GetHashCode();
	}

	public static bool operator ==(Entity<TKey>? left, Entity<TKey>? right)
	{
		return Equals(left, right);
	}

	public static bool operator !=(Entity<TKey>? left, Entity<TKey>? right)
	{
		return !Equals(left, right);
	}
}
