using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Shadowing.ShadowingRole;
using Rakushu.Domain.Entities.Shadowing.ShadowingSession;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingSessionMember;

public interface IShadowingSessionMemberRepository : IBaseRepository<ShadowingSessionMember, ShadowingSessionMemberId>
{
	Task<bool> ExistsAsync(
		ShadowingSessionId sessionId,
		UserId userId,
		CancellationToken cancellationToken = default);

	Task<int> CountBySessionAsync(
		ShadowingSessionId sessionId,
		CancellationToken cancellationToken = default);

	Task<bool> RoleTakenAsync(
		ShadowingSessionId sessionId,
		ShadowingRoleId roleId,
		ShadowingSessionMemberId excludeMemberId,
		CancellationToken cancellationToken = default);
}
