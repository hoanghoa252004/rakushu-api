using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Identity.User;

namespace Rakushu.Domain.Entities.Curation.Report.IssueDetail;

public interface IIssueDetailRepository : IBaseRepository<IssueDetail, IssueDetailId>
{
	Task<IssueDetail?> GetByIdWithReportAsync(
		IssueDetailId id,
		CancellationToken cancellationToken = default);

	Task<bool> HasActiveIssueAsync(
		UserId userId,
		TargetType targetType,
		Guid targetId,
		string category,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyList<IssueDetail>> GetUnassignedByTargetAndCategoryAsync(
		TargetType targetType,
		Guid targetId,
		string category,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyList<IssueDetail>> GetByUserIdAsync(
		UserId userId,
		int page,
		int pageSize,
		CancellationToken cancellationToken = default);
}
