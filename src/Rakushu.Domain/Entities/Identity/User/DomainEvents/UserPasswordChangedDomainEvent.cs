using Rakushu.Domain.Common.Events.DomainEvent;

namespace Rakushu.Domain.Entities.Identity.User.DomainEvents;

public sealed record UserPasswordChangedDomainEvent(User User) : DomainEvent;
