namespace Rakushu.Domain.Common;

public abstract class ValueObject : IEquatable<ValueObject>
{
	// Methods:
	public bool Equals(ValueObject? other)
	{
		if (other is null) return false;
		if (ReferenceEquals(this, other)) return true;
		if (GetType() != other.GetType()) return false;

		return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
	}

	public override bool Equals(object? obj)
	{
		return obj is ValueObject valueObject && Equals(valueObject);
	}

	public override int GetHashCode()
	{
		return GetEqualityComponents()
			.Where(x => x != null)
			.Aggregate(default(int), (hashCode, value) =>
				HashCode.Combine(hashCode, value!.GetHashCode()));
	}

	public static bool operator ==(ValueObject? left, ValueObject? right)
	{
		return Equals(left, right);
	}

	public static bool operator !=(ValueObject? left, ValueObject? right)
	{
		return !Equals(left, right);
	}

	/// <summary>
	/// Nơi khai báo những fields nào quyết định Value Object bằng nhau
	/// </summary>
	/// <returns>
	/// Trả về tất cả các thuộc tính dùng để so sánh bằng nhau của Value Object
	/// </returns>
	protected abstract IEnumerable<object?> GetEqualityComponents();
}
