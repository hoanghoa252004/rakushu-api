using MediatR;

namespace Rakushu.Domain.Common.Events.DomainEvent;

public interface IDomainEvent : INotification
{
	Guid Id { get; }
	DateTime OccurredOn { get; }
}
