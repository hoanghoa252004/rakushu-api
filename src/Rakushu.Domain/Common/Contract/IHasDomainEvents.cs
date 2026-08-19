using Rakushu.Domain.Common.Events.DomainEvent;

namespace Rakushu.Domain.Common.Contract;

public interface IHasDomainEvents
{
	// Properties
	IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

	// Methods
	void AddDomainEvent(IDomainEvent domainEvent);
	void ClearDomainEvents();
}
