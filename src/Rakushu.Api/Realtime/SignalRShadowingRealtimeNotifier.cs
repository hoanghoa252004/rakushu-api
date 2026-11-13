using Microsoft.AspNetCore.SignalR;
using Rakushu.Application.Abstractions.Infrastructure.Realtime;

namespace Rakushu.Api.Realtime;

internal sealed class SignalRShadowingRealtimeNotifier(IHubContext<ShadowingSessionHub> hubContext)
	: IShadowingRealtimeNotifier
{
	public Task NotifyRoleLineCompletedAsync(RoleLineCompletedMessage message, CancellationToken cancellationToken)
	{
		return hubContext.Clients
			.Group(ShadowingSessionHub.GroupName(message.SessionId))
			.SendAsync("RoleLineCompleted", message, cancellationToken);
	}
}
