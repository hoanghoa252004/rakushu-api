using MediatR;
using Rakushu.Domain.Entities.Identity.User.DomainEvents;
using Rakushu.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.DomainEventHandlers.User;

internal sealed class UserBannedDomainEventHandler : INotificationHandler<UserBannedDomainEvent>
{

	public async Task Handle(UserBannedDomainEvent notification, CancellationToken cancellationToken)
	{
		notification.User.RevokeAllActiveRefreshTokens();
	}
}
