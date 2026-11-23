using System.Globalization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Rakushu.Application.Common.Time;
using Rakushu.Application.Usecases.Analytics.Curator;
using Rakushu.Domain.Entities.Curation.Report;

namespace Rakushu.Persistence.Queries;

internal sealed partial class CuratorAnalyticsQueryService
{
	private async Task<CuratorReportsAnalyticsDto> GetReportsSectionAsync(
		int days,
		DateOnly startLocalDate,
		DateOnly localToday,
		DateTimeOffset windowStartUtc,
		TimeZoneInfo timeZone,
		DateTimeOffset utcNow,
		CancellationToken cancellationToken)
	{
		// 1. All-time status counts.
		var statusCounts = await _context.Reports
			.AsNoTracking()
			.GroupBy(r => r.Status)
			.Select(g => new { Status = g.Key, Count = g.Count() })
			.ToListAsync(cancellationToken);

		int pendingCount = statusCounts.FirstOrDefault(x => x.Status == ReportStatus.Pending)?.Count ?? 0;
		int inReviewCount = statusCounts.FirstOrDefault(x => x.Status == ReportStatus.InReview)?.Count ?? 0;
		int resolvedCount = statusCounts.FirstOrDefault(x => x.Status == ReportStatus.Resolved)?.Count ?? 0;
		int dismissedCount = statusCounts.FirstOrDefault(x => x.Status == ReportStatus.Dismissed)?.Count ?? 0;
		int totalReports = pendingCount + inReviewCount + resolvedCount + dismissedCount;

		// 2. Period timestamps for daily trend & period SLA.
		var periodTimestamps = await _context.Reports
			.AsNoTracking()
			.Where(r => r.CreatedAt >= windowStartUtc || (r.ResolvedAt != null && r.ResolvedAt >= windowStartUtc))
			.Select(r => new
			{
				r.Status,
				r.CreatedAt,
				r.ResolvedAt
			})
			.ToListAsync(cancellationToken);

		int periodNewCount = 0;
		int periodResolvedCount = 0;
		var createdByDate = new Dictionary<DateOnly, int>();
		var resolvedByDate = new Dictionary<DateOnly, int>();
		var resolutionHoursSamples = new List<double>();

		foreach (var item in periodTimestamps)
		{
			if (item.CreatedAt >= windowStartUtc)
			{
				periodNewCount++;
				var createdLocalDate = AnalyticsTimeZoneHelper.ToLocalDate(item.CreatedAt, timeZone);
				createdByDate[createdLocalDate] = createdByDate.GetValueOrDefault(createdLocalDate) + 1;
			}

			if (item.ResolvedAt.HasValue
				&& item.ResolvedAt.Value >= windowStartUtc
				&& (item.Status == ReportStatus.Resolved || item.Status == ReportStatus.Dismissed))
			{
				periodResolvedCount++;
				var resolvedLocalDate = AnalyticsTimeZoneHelper.ToLocalDate(item.ResolvedAt.Value, timeZone);
				resolvedByDate[resolvedLocalDate] = resolvedByDate.GetValueOrDefault(resolvedLocalDate) + 1;

				var diffHours = (item.ResolvedAt.Value - item.CreatedAt).TotalHours;
				if (diffHours >= 0)
				{
					resolutionHoursSamples.Add(diffHours);
				}
			}
		}

		double? avgResolutionHours = resolutionHoursSamples.Count > 0
			? Math.Round(resolutionHoursSamples.Average(), 1)
			: null;

		var dailyTrend = new List<CuratorReportDailyTrendPointDto>(days);
		for (var date = startLocalDate; date <= localToday; date = date.AddDays(1))
		{
			dailyTrend.Add(new CuratorReportDailyTrendPointDto(
				date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
				date.ToString("dd/MM", CultureInfo.InvariantCulture),
				createdByDate.GetValueOrDefault(date),
				resolvedByDate.GetValueOrDefault(date)));
		}

		// 3. Category & TargetType breakdowns (all-time backlog composition).
		var rawCategoryBreakdown = await _context.Reports
			.AsNoTracking()
			.GroupBy(r => r.Category)
			.Select(g => new { Category = g.Key, Count = g.Count() })
			.OrderByDescending(x => x.Count)
			.ToListAsync(cancellationToken);

		var categoryBreakdown = rawCategoryBreakdown
			.Select(x => new CuratorReportCategoryBreakdownDto(x.Category, x.Count))
			.ToList();

		var rawTargetTypeCounts = await _context.Reports
			.AsNoTracking()
			.GroupBy(r => r.TargetType)
			.Select(g => new { TargetType = g.Key, Count = g.Count() })
			.ToListAsync(cancellationToken);

		var targetTypeBreakdown = new List<CuratorReportTargetTypeBreakdownDto>
		{
			new(nameof(TargetType.TranscriptSegment), rawTargetTypeCounts.FirstOrDefault(x => x.TargetType == TargetType.TranscriptSegment)?.Count ?? 0),
			new(nameof(TargetType.LearningUnit), rawTargetTypeCounts.FirstOrDefault(x => x.TargetType == TargetType.LearningUnit)?.Count ?? 0),
			new(nameof(TargetType.BankQuestion), rawTargetTypeCounts.FirstOrDefault(x => x.TargetType == TargetType.BankQuestion)?.Count ?? 0)
		};

		// 4. Priority actionable queue (Pending first, oldest SLA first, max 6).
		var priorityRaw = await _context.Reports
			.AsNoTracking()
			.Where(r => r.Status == ReportStatus.Pending || r.Status == ReportStatus.InReview)
			.OrderBy(r => r.Status == ReportStatus.Pending ? 0 : 1)
			.ThenBy(r => r.CreatedAt)
			.Take(PriorityQueueLimit)
			.Select(r => new
			{
				r.Id,
				r.TargetType,
				r.TargetId,
				r.Category,
				r.Status,
				r.TargetSnapshot,
				r.CreatedAt,
				r.UpdatedAt
			})
			.ToListAsync(cancellationToken);

		var priorityReportIds = priorityRaw.Select(r => (ReportId?)r.Id).ToList();
		var issueDetailsLookup = new Dictionary<Guid, (int Count, string? LatestFeedback)>();

		if (priorityReportIds.Count > 0)
		{
			var details = await _context.IssueDetails
				.AsNoTracking()
				.Where(d => d.ReportId != null && priorityReportIds.Contains(d.ReportId))
				.OrderByDescending(d => d.CreatedAt)
				.Select(d => new
				{
					d.ReportId,
					d.LearnerFeedback
				})
				.ToListAsync(cancellationToken);

			foreach (var group in details.Where(d => d.ReportId is not null).GroupBy(d => d.ReportId!.Value))
			{
				var latestFeedback = group
					.Select(d => d.LearnerFeedback)
					.FirstOrDefault(fb => !string.IsNullOrWhiteSpace(fb));

				issueDetailsLookup[group.Key] = (group.Count(), latestFeedback);
			}
		}

		var priorityReports = priorityRaw.Select(r =>
		{
			var detailStats = issueDetailsLookup.GetValueOrDefault(r.Id.Value, (Count: 0, LatestFeedback: (string?)null));
			var ageHours = Math.Max(0d, Math.Round((utcNow - r.CreatedAt).TotalHours, 1));
			return new CuratorPriorityReportItemDto(
				r.Id.Value,
				r.TargetType.ToString(),
				r.TargetId,
				r.Category,
				r.Status.ToString(),
				Math.Max(1, detailStats.Count),
				ExtractSnapshotPreview(r.TargetSnapshot),
				detailStats.LatestFeedback,
				r.CreatedAt,
				r.UpdatedAt,
				ageHours);
		}).ToList();

		return new CuratorReportsAnalyticsDto(
			totalReports,
			pendingCount,
			inReviewCount,
			resolvedCount,
			dismissedCount,
			periodNewCount,
			periodResolvedCount,
			avgResolutionHours,
			dailyTrend,
			categoryBreakdown,
			targetTypeBreakdown,
			priorityReports);
	}

	private static string? ExtractSnapshotPreview(string? snapshotJson)
	{
		if (string.IsNullOrWhiteSpace(snapshotJson))
			return null;

		var trimmed = snapshotJson.Trim();
		if (!trimmed.StartsWith('{'))
		{
			return TruncateText(trimmed, 120);
		}

		try
		{
			using var doc = JsonDocument.Parse(trimmed);
			var root = doc.RootElement;
			string[] candidateKeys =
			[
				"originalSentenceJp",
				"expression",
				"question",
				"questionText",
				"prompt",
				"text",
				"segmentText",
				"surface",
				"meaning",
				"subtitleVn",
				"videoTitle",
				"title",
				"content"
			];

			foreach (var key in candidateKeys)
			{
				if (root.TryGetProperty(key, out var prop) && prop.ValueKind == JsonValueKind.String)
				{
					var value = prop.GetString();
					if (!string.IsNullOrWhiteSpace(value))
					{
						return TruncateText(value.Trim(), 120);
					}
				}
			}
		}
		catch (JsonException)
		{
			return TruncateText(trimmed, 120);
		}

		return null;
	}

	private static string TruncateText(string text, int maxLength)
		=> text.Length <= maxLength ? text : string.Concat(text.AsSpan(0, maxLength - 1), "…");
}
