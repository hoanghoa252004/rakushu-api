using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Curation.Report;
using Rakushu.Domain.Entities.Curation.Report.IssueDetail;
using Rakushu.Domain.Entities.Identity.User;

namespace Rakushu.Persistence.Repositories;

public sealed class IssueDetailRepository : BaseRepository<IssueDetail, IssueDetailId>, IIssueDetailRepository
{
	public IssueDetailRepository(RakushuDbContext context) : base(context)
	{
	}

	public async Task<IssueDetail?> GetByIdWithReportAsync(
		IssueDetailId id,
		CancellationToken cancellationToken = default)
	{
		return await _context.IssueDetails
			.Include(x => x.Report)
			.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
	}

	public async Task<bool> HasActiveIssueAsync(
		UserId userId,
		TargetType targetType,
		Guid targetId,
		string category,
		CancellationToken cancellationToken = default)
	{
		return await _context.IssueDetails
			.AnyAsync(
				id => id.UserId == userId &&
					  id.TargetType == targetType &&
					  id.TargetId == targetId &&
					  id.Category == category &&
					  (id.ReportId == null || (id.Report != null && id.Report.Status != ReportStatus.Resolved && id.Report.Status != ReportStatus.Dismissed)),
				cancellationToken);
	}

	public async Task<IReadOnlyList<IssueDetail>> GetUnassignedByTargetAndCategoryAsync(
		TargetType targetType,
		Guid targetId,
		string category,
		CancellationToken cancellationToken = default)
	{
		return await _context.IssueDetails
			.Where(id => id.TargetType == targetType &&
						 id.TargetId == targetId &&
						 id.Category == category &&
						 id.ReportId == null)
			.ToListAsync(cancellationToken);
	}

	public async Task<(IReadOnlyList<IssueDetail> Items, int TotalCount)> GetByUserIdAsync(
		UserId userId,
		int page,
		int pageSize,
		string? status = null,
		TargetType? targetType = null,
		CancellationToken cancellationToken = default)
	{
		var query = _context.IssueDetails
			.Include(id => id.Report)
			.Where(id => id.UserId == userId);

		if (!string.IsNullOrWhiteSpace(status))
		{
			string normalizedStatus = status.Trim().ToLowerInvariant();
			query = normalizedStatus switch
			{
				"pending" => query.Where(id =>
					id.ReportId == null ||
					(id.Report != null && id.Report.Status == ReportStatus.Pending)),
				"investigating" or "inreview" or "in_review" => query.Where(id =>
					id.Report != null && id.Report.Status == ReportStatus.InReview),
				"resolved" => query.Where(id =>
					id.Report != null && id.Report.Status == ReportStatus.Resolved),
				"dismissed" => query.Where(id =>
					id.Report != null && id.Report.Status == ReportStatus.Dismissed),
				_ => query
			};
		}

		if (targetType.HasValue)
		{
			query = query.Where(id => id.TargetType == targetType.Value);
		}

		var totalCount = await query.CountAsync(cancellationToken);

		var items = await query
			.OrderByDescending(id => id.CreatedAt)
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync(cancellationToken);

		return (items, totalCount);
	}

	public async Task<(int TotalCount, int PendingCount, int InvestigatingCount, int ResolvedCount, int DismissedCount)> GetSummaryByUserIdAsync(
		UserId userId,
		CancellationToken cancellationToken = default)
	{
		var grouped = await _context.IssueDetails
			.Where(id => id.UserId == userId)
			.GroupBy(id => id.Report != null ? (ReportStatus?)id.Report.Status : null)
			.Select(g => new { Status = g.Key, Count = g.Count() })
			.ToListAsync(cancellationToken);

		int pending = 0, investigating = 0, resolved = 0, dismissed = 0;
		foreach (var row in grouped)
		{
			switch (row.Status)
			{
				case null:
				case ReportStatus.Pending:
					pending += row.Count;
					break;
				case ReportStatus.InReview:
					investigating += row.Count;
					break;
				case ReportStatus.Resolved:
					resolved += row.Count;
					break;
				case ReportStatus.Dismissed:
					dismissed += row.Count;
					break;
			}
		}

		int total = pending + investigating + resolved + dismissed;
		return (total, pending, investigating, resolved, dismissed);
	}
}
