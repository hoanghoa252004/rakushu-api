using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Realtime;
using Rakushu.Domain.Entities.Shadowing.ShadowingAttempt.ShadowingAttemptDetail;

namespace Rakushu.Application.DomainEventHandlers.Shadowing;

internal sealed class AttemptDetailCreatedDomainEventHandler(IShadowingRealtimeNotifier notifier)
	: INotificationHandler<AttemptDetailCreatedDomainEvent>
{
	public Task Handle(AttemptDetailCreatedDomainEvent notification, CancellationToken cancellationToken)
	{
		return notifier.NotifyRoleLineCompletedAsync(
			new RoleLineCompletedMessage(
				notification.SessionId,
				notification.AttemptId,
				notification.RoleLineId,
				notification.MemberId,
				notification.IsSkipped,
				notification.CompletedAt),
			cancellationToken);
	}
}
