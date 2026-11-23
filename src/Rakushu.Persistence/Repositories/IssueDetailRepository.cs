using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Curation.Report;
using Rakushu.Domain.Entities.Curation.Report.IssueDetail;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Material.Video;

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
		int? startCharIndex = null,
		int? endCharIndex = null,
		CancellationToken cancellationToken = default)
	{
		return await _context.IssueDetails
			.AnyAsync(
				id => id.UserId == userId &&
					  id.TargetType == targetType &&
					  id.TargetId == targetId &&
					  id.Category == category &&
					  id.StartCharIndex == startCharIndex &&
					  id.EndCharIndex == endCharIndex &&
					  (id.ReportId == null || (id.Report != null && id.Report.Status != ReportStatus.Resolved && id.Report.Status != ReportStatus.Dismissed)),
				cancellationToken);
	}

	public async Task<bool> HasAnyIssueAsync(
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
					  id.Category == category,
				cancellationToken);
	}

	public async Task<int> CountByReportIdAsync(
		ReportId reportId,
		CancellationToken cancellationToken = default)
	{
		return await _context.IssueDetails
			.CountAsync(id => id.ReportId == reportId, cancellationToken);
	}

	public async Task<IssueDetail?> GetLatestByTargetAndCategoryAsync(
		TargetType targetType,
		Guid targetId,
		string category,
		UserId? userId = null,
		CancellationToken cancellationToken = default)
	{
		var query = _context.IssueDetails
			.AsNoTracking()
			.Include(id => id.Report)
			.Where(id => id.TargetType == targetType &&
						 id.TargetId == targetId &&
						 id.Category == category);

		if (userId is not null)
		{
			query = query.Where(id => id.UserId == userId);
		}

		return await query
			.OrderByDescending(id => id.CreatedAt)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<IssueDetail>> GetUnassignedByTargetAndCategoryAsync(
		TargetType targetType,
		Guid targetId,
		string category,
		int? startCharIndex = null,
		int? endCharIndex = null,
		CancellationToken cancellationToken = default)
	{
		return await _context.IssueDetails
			.Where(id => id.TargetType == targetType &&
						 id.TargetId == targetId &&
						 id.Category == category &&
						 id.StartCharIndex == startCharIndex &&
						 id.EndCharIndex == endCharIndex &&
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

	public async Task<(IReadOnlyList<CuratedOovStatRow> Items, int TotalCount)> GetCuratedOovStatsAsync(
		Guid? videoId,
		string? status,
		string? search,
		int page,
		int pageSize,
		CancellationToken cancellationToken = default)
	{
		var query = _context.IssueDetails
			.AsNoTracking()
			.Where(d =>
				d.TargetType == TargetType.TranscriptSegment &&
				d.Category == "Oov" &&
				d.StartCharIndex != null &&
				d.EndCharIndex != null);

		string normalizedStatus = string.IsNullOrWhiteSpace(status)
			? "open"
			: status.Trim().ToLowerInvariant();

		query = normalizedStatus switch
		{
			"all" => query,
			"collecting" => query.Where(d => d.ReportId == null),
			"pending" => query.Where(d => d.Report != null && d.Report.Status == ReportStatus.Pending),
			"inreview" or "in_review" => query.Where(d => d.Report != null && d.Report.Status == ReportStatus.InReview),
			"resolved" => query.Where(d => d.Report != null && d.Report.Status == ReportStatus.Resolved),
			"dismissed" => query.Where(d => d.Report != null && d.Report.Status == ReportStatus.Dismissed),
			_ => query.Where(d =>
				d.ReportId == null ||
				(d.Report != null && (d.Report.Status == ReportStatus.Pending || d.Report.Status == ReportStatus.InReview)))
		};

		var rawIssues = await query
			.Select(d => new
			{
				d.TargetId,
				StartCharIndex = d.StartCharIndex!.Value,
				EndCharIndex = d.EndCharIndex!.Value,
				UserId = d.UserId.Value,
				ReportId = d.ReportId != null ? d.ReportId.Value : (Guid?)null,
				ReportStatus = d.Report != null ? (ReportStatus?)d.Report.Status : null,
				d.LearnerFeedback,
				d.TargetSnapshot,
				d.CreatedAt
			})
			.ToListAsync(cancellationToken);

		if (rawIssues.Count == 0)
		{
			return ([], 0);
		}

		var distinctSegIds = rawIssues
			.Select(r => TranscriptSegmentId.From(r.TargetId))
			.Distinct()
			.ToArray();

		var segQuery = _context.TranscriptSegments
			.AsNoTracking()
			.Where(seg =>
				distinctSegIds.Contains(seg.Id) &&
				seg.Transcript.Video.SourceType == VideoSource.Curated);

		if (videoId.HasValue && videoId.Value != Guid.Empty)
		{
			var typedVideoId = VideoId.From(videoId.Value);
			segQuery = segQuery.Where(seg => seg.Transcript.VideoId == typedVideoId);
		}

		var curatedSegments = await segQuery
			.Select(seg => new
			{
				SegmentId = seg.Id.Value,
				VideoId = seg.Transcript.Video.Id.Value,
				VideoTitle = seg.Transcript.Video.Title,
				StartTimeSeconds = seg.StartTime.TotalSeconds,
				SentenceJp = seg.Text,
				SubtitleVn = seg.Subtitle != null ? seg.Subtitle.Text : null
			})
			.ToDictionaryAsync(s => s.SegmentId, cancellationToken);

		if (curatedSegments.Count == 0)
		{
			return ([], 0);
		}

		var normalizedSearch = string.IsNullOrWhiteSpace(search) ? null : search.Trim();

		var grouped = rawIssues
			.Where(r => curatedSegments.ContainsKey(r.TargetId))
			.GroupBy(r => (r.TargetId, r.StartCharIndex, r.EndCharIndex))
			.Select(g =>
			{
				var seg = curatedSegments[g.Key.TargetId];
				var ordered = g.OrderByDescending(x => x.CreatedAt).ToList();

				var activeEntry = ordered.FirstOrDefault(x =>
					x.ReportId.HasValue &&
					(x.ReportStatus == ReportStatus.InReview || x.ReportStatus == ReportStatus.Pending));
				var latestReportEntry = activeEntry ?? ordered.FirstOrDefault(x => x.ReportId.HasValue);

				string groupStatus = latestReportEntry?.ReportStatus switch
				{
					ReportStatus.InReview => "InReview",
					ReportStatus.Pending => "Pending",
					ReportStatus.Resolved => "Resolved",
					ReportStatus.Dismissed => "Dismissed",
					_ => "Collecting"
				};

				var sampleFeedbacks = ordered
					.Select(x => x.LearnerFeedback)
					.Where(f => !string.IsNullOrWhiteSpace(f))
					.Select(f => f!.Trim())
					.Distinct()
					.Take(3)
					.ToList();

				var earliestSnapshot = ordered
					.LastOrDefault(x => !string.IsNullOrWhiteSpace(x.TargetSnapshot))
					?.TargetSnapshot;

				return new CuratedOovStatRow(
					seg.VideoId,
					seg.VideoTitle ?? "Video bài học",
					seg.SegmentId,
					seg.StartTimeSeconds,
					seg.SentenceJp ?? string.Empty,
					seg.SubtitleVn,
					g.Key.StartCharIndex,
					g.Key.EndCharIndex,
					earliestSnapshot,
					ordered.Count,
					ordered.Select(x => x.UserId).Distinct().Count(),
					activeEntry?.ReportId ?? latestReportEntry?.ReportId,
					groupStatus,
					ordered[^1].CreatedAt,
					ordered[0].CreatedAt,
					sampleFeedbacks);
			})
			.ToList();

		if (normalizedSearch is not null)
		{
			grouped = grouped
				.Where(item =>
				{
					string oovSlice = item.StartCharIndex >= 0 &&
						item.EndCharIndex > item.StartCharIndex &&
						item.EndCharIndex <= item.SentenceJp.Length
						? item.SentenceJp.Substring(item.StartCharIndex, item.EndCharIndex - item.StartCharIndex)
						: string.Empty;

					return item.VideoTitle.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
						   item.SentenceJp.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
						   oovSlice.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase);
				})
				.ToList();
		}

		var sorted = grouped
			.OrderByDescending(x => x.ReportCount)
			.ThenByDescending(x => x.UniqueLearnerCount)
			.ThenByDescending(x => x.LastReportedAt)
			.ToList();

		int totalCount = sorted.Count;
		var pagedItems = sorted
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.ToList();

		return (pagedItems, totalCount);
	}
}
