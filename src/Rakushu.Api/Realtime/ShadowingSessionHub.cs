using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Rakushu.Domain.Entities.Shadowing.ShadowingSession;

namespace Rakushu.Api.Realtime;

[Authorize]
public sealed class ShadowingSessionHub(IShadowingSessionRepository sessionRepository) : Hub
{
	public const string Route = "/hubs/shadowing-sessions";

	public static string GroupName(Guid sessionId) => $"shadowing-session:{sessionId}";

	public async Task JoinSession(Guid sessionId)
	{
		if (!Guid.TryParse(Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
			throw new HubException("Unauthenticated.");

		var session = await sessionRepository.GetByIdAsync(
			ShadowingSessionId.From(sessionId), Context.ConnectionAborted);
		if (session is null)
			throw new HubException("Session not found.");
		if (!session.Members.Any(m => m.MemberId.Value == userId))
			throw new HubException("You are not a member of this session.");

		await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(sessionId));
	}

	public Task LeaveSession(Guid sessionId)
		=> Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(sessionId));
}
