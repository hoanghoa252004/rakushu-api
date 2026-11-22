using Microsoft.EntityFrameworkCore;
using Rakushu.Application.Abstractions.Persistence.Queries;
using Rakushu.Application.Common.Time;
using Rakushu.Application.Usecases.Analytics.Curator;
using Rakushu.Domain.Entities.Curation.Report;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Srs.BankQuestion;

namespace Rakushu.Persistence.Queries;

internal sealed partial class CuratorAnalyticsQueryService : ICuratorAnalyticsQueryService
{
	private const double LowConfidenceThreshold = 0.70d;
	private const int PriorityQueueLimit = 6;

	private readonly RakushuDbContext _context;

	public CuratorAnalyticsQueryService(RakushuDbContext context)
	{
		_context = context;
	}

	public async Task<CuratorDashboardDto> GetCuratorDashboardAsync(
		int days,
		TimeZoneInfo timeZone,
		DateTimeOffset utcNow,
		CancellationToken cancellationToken = default)
	{
		// RakushuDbContext is scoped and not thread-safe: execute sections sequentially.
		var localToday = AnalyticsTimeZoneHelper.GetLocalToday(utcNow, timeZone);
		var startLocalDate = localToday.AddDays(-(days - 1));
		var (windowStartUtc, _) = AnalyticsTimeZoneHelper.GetUtcBoundsForLocalDate(startLocalDate, timeZone);

		var reports = await GetReportsSectionAsync(
			days,
			startLocalDate,
			localToday,
			windowStartUtc,
			timeZone,
			utcNow,
			cancellationToken);

		var videos = await GetVideosSectionAsync(
			windowStartUtc,
			cancellationToken);

		var questionBank = await GetQuestionBankSectionAsync(
			windowStartUtc,
			cancellationToken);

		var knowledge = await GetKnowledgeSectionAsync(
			windowStartUtc,
			videos.LowConfidenceLearningUnitsCount,
			cancellationToken);

		return new CuratorDashboardDto(
			days,
			timeZone.Id,
			utcNow,
			reports,
			videos,
			questionBank,
			knowledge);
	}

	public async Task<CuratorSummaryDto> GetCuratorSummaryAsync(
		DateTimeOffset utcNow,
		CancellationToken cancellationToken = default)
	{
		var reportStatusCounts = await _context.Reports
			.AsNoTracking()
			.GroupBy(r => r.Status)
			.Select(g => new { Status = g.Key, Count = g.Count() })
			.ToListAsync(cancellationToken);

		int reportPending = reportStatusCounts.FirstOrDefault(x => x.Status == ReportStatus.Pending)?.Count ?? 0;
		int reportInReview = reportStatusCounts.FirstOrDefault(x => x.Status == ReportStatus.InReview)?.Count ?? 0;
		int reportResolved = reportStatusCounts.FirstOrDefault(x => x.Status == ReportStatus.Resolved)?.Count ?? 0;
		int reportDismissed = reportStatusCounts.FirstOrDefault(x => x.Status == ReportStatus.Dismissed)?.Count ?? 0;

		var reportsSummary = new CuratorReportsSummaryCountsDto(
			reportPending + reportInReview + reportResolved + reportDismissed,
			reportPending,
			reportInReview,
			reportResolved,
			reportDismissed);

		var videoStatusCounts = await _context.Videos
			.AsNoTracking()
			.GroupBy(v => v.Status)
			.Select(g => new { Status = g.Key, Count = g.Count() })
			.ToListAsync(cancellationToken);

		int videoReady = videoStatusCounts.FirstOrDefault(x => x.Status == VideoStatus.Ready)?.Count ?? 0;
		int videoReviewing = videoStatusCounts.FirstOrDefault(x => x.Status == VideoStatus.Reviewing)?.Count ?? 0;
		int videoPending = videoStatusCounts.FirstOrDefault(x => x.Status == VideoStatus.Pending)?.Count ?? 0;
		int videoProcessing = videoStatusCounts.FirstOrDefault(x => x.Status == VideoStatus.Processing)?.Count ?? 0;
		int videoDraft = videoStatusCounts.FirstOrDefault(x => x.Status == VideoStatus.Draft)?.Count ?? 0;
		int videoRejected = videoStatusCounts.FirstOrDefault(x => x.Status == VideoStatus.Rejected)?.Count ?? 0;
		int videoArchived = videoStatusCounts.FirstOrDefault(x => x.Status == VideoStatus.Archived)?.Count ?? 0;

		var videosSummary = new CuratorVideosSummaryCountsDto(
			videoReady + videoReviewing + videoPending + videoProcessing + videoDraft + videoRejected + videoArchived,
			videoReady,
			videoReviewing,
			videoPending,
			videoProcessing,
			videoDraft,
			videoRejected,
			videoArchived);

		var questionGroups = await _context.BankQuestions
			.AsNoTracking()
			.GroupBy(q => new { q.Status, q.IsActive, q.MediaType })
			.Select(g => new
			{
				g.Key.Status,
				g.Key.IsActive,
				g.Key.MediaType,
				Count = g.Count()
			})
			.ToListAsync(cancellationToken);

		int publishedCount = 0;
		int draftCount = 0;
		int inactiveCount = 0;
		int withAudio = 0;
		int withVideo = 0;
		int withImage = 0;
		int textOnly = 0;

		foreach (var group in questionGroups)
		{
			if (group.Status == QuestionStatus.Published && !group.IsActive)
			{
				inactiveCount += group.Count;
				continue;
			}

			if (group.Status == QuestionStatus.Draft)
			{
				draftCount += group.Count;
			}
			else if (group.Status == QuestionStatus.Published && group.IsActive)
			{
				publishedCount += group.Count;
			}

			var mediaType = (group.MediaType ?? "NONE").Trim().ToUpperInvariant();
			switch (mediaType)
			{
				case "AUDIO":
					withAudio += group.Count;
					break;
				case "VIDEO":
					withVideo += group.Count;
					break;
				case "IMAGE":
					withImage += group.Count;
					break;
				default:
					textOnly += group.Count;
					break;
			}
		}

		var questionBankSummary = new CuratorQuestionBankSummaryCountsDto(
			publishedCount + draftCount,
			publishedCount,
			draftCount,
			inactiveCount,
			withAudio + withVideo + withImage,
			withAudio,
			withVideo,
			withImage,
			textOnly);

		var collectionCounts = await _context.KnowledgeCollections
			.AsNoTracking()
			.GroupBy(c => c.IsActive)
			.Select(g => new { IsActive = g.Key, Count = g.Count() })
			.ToListAsync(cancellationToken);

		int activeCollections = collectionCounts.FirstOrDefault(x => x.IsActive)?.Count ?? 0;
		int inactiveCollections = collectionCounts.FirstOrDefault(x => !x.IsActive)?.Count ?? 0;

		var unitCounts = await _context.KnowledgeUnits
			.AsNoTracking()
			.GroupBy(u => new { u.SourceType, u.IsActive })
			.Select(g => new { g.Key.SourceType, g.Key.IsActive, Count = g.Count() })
			.ToListAsync(cancellationToken);

		int activeUnits = unitCounts.Where(x => x.IsActive).Sum(x => x.Count);
		int pendingUnits = unitCounts.Where(x => !x.IsActive).Sum(x => x.Count);
		int oovTotal = unitCounts.Where(x => x.SourceType == KnowledgeUnitSourceType.OovDetected).Sum(x => x.Count);
		int oovPending = unitCounts.Where(x => x.SourceType == KnowledgeUnitSourceType.OovDetected && !x.IsActive).Sum(x => x.Count);

		var knowledgeSummary = new CuratorKnowledgeSummaryCountsDto(
			activeCollections + inactiveCollections,
			activeCollections,
			inactiveCollections,
			activeUnits + pendingUnits,
			activeUnits,
			pendingUnits,
			oovTotal,
			oovPending);

		return new CuratorSummaryDto(
			utcNow,
			reportsSummary,
			videosSummary,
			questionBankSummary,
			knowledgeSummary);
	}
}
