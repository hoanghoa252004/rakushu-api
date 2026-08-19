namespace Rakushu.Domain.Common;

public abstract class StronglyTypedId<TValue> : ValueObject
	where TValue : notnull
{
	// Constructors
	protected StronglyTypedId(TValue value)
	{
		Value = value;
	}

	// Properties
	public TValue Value { get; }

	// Methods
	protected override IEnumerable<object?> GetEqualityComponents()
	{
		yield return Value;
	}

	public override string ToString() => Value.ToString() ?? string.Empty;
}

