namespace Rakushu.Domain.Common.Events.DomainEvent;

public abstract record DomainEvent : IDomainEvent
{
	private protected DomainEvent()
	{
		Id = Guid.NewGuid();
		OccurredOn = DateTime.UtcNow;
	}

	public Guid Id { get; }
	public DateTime OccurredOn { get; }
}
