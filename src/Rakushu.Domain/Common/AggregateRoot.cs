using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Events.DomainEvent;

namespace Rakushu.Domain.Common;

public abstract class AggregateRoot<TKey> : Entity<TKey>, IHasDomainEvents
	where TKey : notnull
{
	// Fields
	private readonly List<IDomainEvent> _domainEvents = [];

	// Constructors
	protected AggregateRoot() : base() { }

	protected AggregateRoot(TKey id) : base(id) { }

	// Properties
	public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

	// Methods
	public void ClearDomainEvents()
	{
		_domainEvents.Clear();
	}

	public void AddDomainEvent(IDomainEvent domainEvent)
	{
		_domainEvents.Add(domainEvent);
	}

}
