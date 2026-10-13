using Rakushu.Domain.Common.Events.DomainEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Identity.User.DomainEvents;

public sealed record UserBannedDomainEvent(User User) : DomainEvent;