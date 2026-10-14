using MediatR;
using Rakushu.Domain.Entities.Identity.User.DomainEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.DomainEventHandlers.User;

internal sealed class UserPasswordChangedDomainEventHandler : INotificationHandler<UserPasswordChangedDomainEvent>
{

	public async Task Handle(UserPasswordChangedDomainEvent notification, CancellationToken cancellationToken)
	{
		notification.User.RevokeAllActiveRefreshTokens();
	}
}
