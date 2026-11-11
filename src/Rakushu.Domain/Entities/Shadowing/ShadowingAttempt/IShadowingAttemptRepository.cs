using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Shadowing.ShadowingAttempt.ShadowingAttemptDetail;
using Rakushu.Domain.Entities.Shadowing.ShadowingRoleLine;
using Rakushu.Domain.Entities.Shadowing.ShadowingSession;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingAttempt;

public interface IShadowingAttemptRepository : IBaseRepository<ShadowingAttempt, ShadowingAttemptId>
{
	// Tracked attempt with ShadowingSession and ShadowingSessionMember loaded.
	Task<ShadowingAttempt?> GetWithContextAsync(
		ShadowingAttemptId id,
		CancellationToken cancellationToken = default);

	Task<ShadowingAttempt?> GetByMemberAsync(
		ShadowingSessionId sessionId,
		UserId userId,
		CancellationToken cancellationToken = default);

	Task<ShadowingAttemptDetail.ShadowingAttemptDetail?> GetDetailAsync(
		ShadowingAttemptId attemptId,
		ShadowingRoleLineId roleLineId,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyList<ShadowingAttemptDetail.ShadowingAttemptDetail>> GetDetailsAsync(
		ShadowingAttemptId attemptId,
		CancellationToken cancellationToken = default);

	Task<bool> RoleLineDoneInSessionAsync(
		ShadowingSessionId sessionId,
		ShadowingRoleLineId roleLineId,
		CancellationToken cancellationToken = default);

	Task<int> CountSkippedDetailsAsync(ShadowingAttemptId attemptId, CancellationToken cancellationToken = default);

	Task<int> CountDetailsAsync(
		ShadowingAttemptId attemptId,
		CancellationToken cancellationToken = default);

	Task<(IReadOnlyList<ShadowingAttemptDetail.ShadowingAttemptDetail> Items, int TotalCount)> GetDetailsPagedAsync(
		ShadowingAttemptId attemptId,
		int pageNumber,
		int pageSize,
		CancellationToken cancellationToken = default,
		bool? isSkipped = null);

	Task<bool> IsRoleAttemptCancelledAsync(
		ShadowingSessionId sessionId,
		ShadowingRole.ShadowingRoleId roleId,
		CancellationToken cancellationToken = default);

	void AddDetail(ShadowingAttemptDetail.ShadowingAttemptDetail detail);

	// Tracked Completed attempts whose session is Completed, with Details and their RoleLine loaded.
	Task<IReadOnlyList<ShadowingAttempt>> GetPendingAssessmentAsync(
		int batchSize,
		CancellationToken cancellationToken = default);

	void AddAssessment(ShadowingAssessment.ShadowingAssessment assessment);

	// Attempts of every member of the session (ShadowingSessionMember loaded).
	Task<IReadOnlyList<ShadowingAttempt>> GetBySessionAsync(
		ShadowingSessionId sessionId,
		CancellationToken cancellationToken = default);

	// Details of any member in the session for the given role lines (ShadowingAttempt loaded).
	Task<IReadOnlyList<ShadowingAttemptDetail.ShadowingAttemptDetail>> GetSessionDetailsAsync(
		ShadowingSessionId sessionId,
		IReadOnlyCollection<ShadowingRoleLineId> roleLineIds,
		CancellationToken cancellationToken = default);
}
