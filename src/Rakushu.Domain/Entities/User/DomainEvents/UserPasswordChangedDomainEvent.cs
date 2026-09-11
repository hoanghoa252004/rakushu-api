using Rakushu.Domain.Common.Events.DomainEvent;

namespace Rakushu.Domain.Entities.User.DomainEvents;

public sealed record UserPasswordChangedDomainEvent(User User) : DomainEvent;
