using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Shadowing.ShadowingRole;
using Rakushu.Domain.Entities.Shadowing.ShadowingSession;
using Rakushu.Domain.Entities.Shadowing.ShadowingSessionMember;

namespace Rakushu.Persistence.Repositories;

public sealed class ShadowingSessionMemberRepository
	: BaseRepository<ShadowingSessionMember, ShadowingSessionMemberId>, IShadowingSessionMemberRepository
{
	public ShadowingSessionMemberRepository(RakushuDbContext context) : base(context) { }

	public async Task<bool> ExistsAsync(
		ShadowingSessionId sessionId, UserId userId, CancellationToken cancellationToken = default)
	{
		return await _context.ShadowingSessionMembers.AnyAsync(
			m => m.ShadowingSessionId == sessionId && m.MemberId == userId, cancellationToken);
	}

	public async Task<int> CountBySessionAsync(
		ShadowingSessionId sessionId, CancellationToken cancellationToken = default)
	{
		return await _context.ShadowingSessionMembers.CountAsync(
			m => m.ShadowingSessionId == sessionId, cancellationToken);
	}

	public async Task<bool> RoleTakenAsync(
		ShadowingSessionId sessionId,
		ShadowingRoleId roleId,
		ShadowingSessionMemberId excludeMemberId,
		CancellationToken cancellationToken = default)
	{
		return await _context.ShadowingSessionMembers.AnyAsync(
			m => m.ShadowingSessionId == sessionId
				&& m.ShadowingRoleId == roleId
				&& m.Id != excludeMemberId,
			cancellationToken);
	}
}
