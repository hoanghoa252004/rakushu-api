using Rakushu.Domain.Common.Events.DomainEvent;

namespace Rakushu.Domain.Entities.User.Events;

public sealed record UserPasswordChangedDomainEvent(Guid UserId) : DomainEvent;
