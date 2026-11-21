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
		int? startCharIndex = null,
		int? endCharIndex = null,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyList<IssueDetail>> GetUnassignedByTargetAndCategoryAsync(
		TargetType targetType,
		Guid targetId,
		string category,
		int? startCharIndex = null,
		int? endCharIndex = null,
		CancellationToken cancellationToken = default);

	Task<(IReadOnlyList<IssueDetail> Items, int TotalCount)> GetByUserIdAsync(
		UserId userId,
		int page,
		int pageSize,
		string? status = null,
		TargetType? targetType = null,
		CancellationToken cancellationToken = default);

	Task<(int TotalCount, int PendingCount, int InvestigatingCount, int ResolvedCount, int DismissedCount)> GetSummaryByUserIdAsync(
		UserId userId,
		CancellationToken cancellationToken = default);

	Task<(IReadOnlyList<CuratedOovStatRow> Items, int TotalCount)> GetCuratedOovStatsAsync(
		Guid? videoId,
		string? status,
		string? search,
		int page,
		int pageSize,
		CancellationToken cancellationToken = default);
}

public sealed record CuratedOovStatRow(
	Guid VideoId,
	string VideoTitle,
	Guid SegmentId,
	double StartTimeSeconds,
	string SentenceJp,
	string? SubtitleVn,
	int StartCharIndex,
	int EndCharIndex,
	string? SnapshotJson,
	int ReportCount,
	int UniqueLearnerCount,
	Guid? ActiveReportId,
	string ReportStatus,
	DateTimeOffset FirstReportedAt,
	DateTimeOffset LastReportedAt,
	IReadOnlyList<string> SampleFeedbacks);
