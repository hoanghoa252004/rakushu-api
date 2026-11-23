using Microsoft.EntityFrameworkCore;
using Rakushu.Application.Usecases.Analytics.Curator;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Persistence.Queries;

internal sealed partial class CuratorAnalyticsQueryService
{
	private static readonly VideoStatus[] OrderedVideoStatuses =
	[
		VideoStatus.Ready,
		VideoStatus.Reviewing,
		VideoStatus.Pending,
		VideoStatus.Processing,
		VideoStatus.Draft,
		VideoStatus.Rejected,
		VideoStatus.Archived
	];

	private async Task<CuratorVideosAnalyticsDto> GetVideosSectionAsync(
		DateTimeOffset windowStartUtc,
		CancellationToken cancellationToken)
	{
		// 1. All-time status counts (all 7 VideoStatus values).
		var rawStatusCounts = await _context.Videos
			.AsNoTracking()
			.GroupBy(v => v.Status)
			.Select(g => new { Status = g.Key, Count = g.Count() })
			.ToListAsync(cancellationToken);

		var statusMap = rawStatusCounts.ToDictionary(x => x.Status, x => x.Count);
		int readyCount = statusMap.GetValueOrDefault(VideoStatus.Ready);
		int reviewingCount = statusMap.GetValueOrDefault(VideoStatus.Reviewing);
		int pendingCount = statusMap.GetValueOrDefault(VideoStatus.Pending);
		int processingCount = statusMap.GetValueOrDefault(VideoStatus.Processing);
		int draftCount = statusMap.GetValueOrDefault(VideoStatus.Draft);
		int rejectedCount = statusMap.GetValueOrDefault(VideoStatus.Rejected);
		int archivedCount = statusMap.GetValueOrDefault(VideoStatus.Archived);
		int totalVideos = readyCount + reviewingCount + pendingCount + processingCount + draftCount + rejectedCount + archivedCount;

		var statusBreakdown = OrderedVideoStatuses
			.Select(status => new CuratorStatusBreakdownDto(status.ToString(), statusMap.GetValueOrDefault(status)))
			.ToList();

		// 2. Source type counts.
		var rawSourceCounts = await _context.Videos
			.AsNoTracking()
			.GroupBy(v => v.SourceType)
			.Select(g => new { SourceType = g.Key, Count = g.Count() })
			.ToListAsync(cancellationToken);

		int curatedCount = rawSourceCounts.FirstOrDefault(x => x.SourceType == VideoSource.Curated)?.Count ?? 0;
		int learnerUploadedCount = rawSourceCounts.FirstOrDefault(x => x.SourceType == VideoSource.UploadedByLearner)?.Count ?? 0;
		int externalImportedCount = rawSourceCounts.FirstOrDefault(x => x.SourceType == VideoSource.ImportedFromExternal)?.Count ?? 0;

		// 3. Period activity counts.
		int periodAddedCount = await _context.Videos
			.AsNoTracking()
			.CountAsync(v => v.CreatedAt >= windowStartUtc, cancellationToken);

		int periodReadyCount = await _context.Videos
			.AsNoTracking()
			.CountAsync(v => v.Status == VideoStatus.Ready && v.UpdatedAt >= windowStartUtc, cancellationToken);

		// 4. Real curation quality metrics across transcripts and learning units.
		int unreviewedTranscriptsCount = await _context.Transcripts
			.AsNoTracking()
			.CountAsync(t => !t.IsReviewed, cancellationToken);

		int unreviewedLearningUnitsCount = await _context.LearningUnits
			.AsNoTracking()
			.CountAsync(lu => !lu.IsReviewed, cancellationToken);

		int lowConfidenceLearningUnitsCount = await _context.LearningUnits
			.AsNoTracking()
			.CountAsync(lu => lu.Confidence < LowConfidenceThreshold, cancellationToken);

		// 5. Needs-attention queue: strictly Reviewing or Pending (never padded with Ready).
		var queueRaw = await _context.Videos
			.AsNoTracking()
			.Where(v => v.Status == VideoStatus.Reviewing || v.Status == VideoStatus.Pending)
			.OrderBy(v => v.Status == VideoStatus.Pending ? 0 : 1)
			.ThenByDescending(v => v.UpdatedAt)
			.Take(PriorityQueueLimit)
			.Select(v => new
			{
				v.Id,
				v.Title,
				v.Status,
				v.SourceType,
				ContentCategoryName = v.ContentCategory != null ? v.ContentCategory.Name : null,
				v.Duration,
				v.CreatedAt,
				v.UpdatedAt
			})
			.ToListAsync(cancellationToken);

		var queueVideoIds = queueRaw.Select(v => v.Id).ToList();
		var unreviewedUnitsByVideo = new Dictionary<Guid, int>();

		if (queueVideoIds.Count > 0)
		{
			var rawUnitCounts = await _context.LearningUnits
				.AsNoTracking()
				.Where(lu => !lu.IsReviewed && queueVideoIds.Contains(lu.TranscriptSegment.Transcript.VideoId))
				.GroupBy(lu => lu.TranscriptSegment.Transcript.VideoId)
				.Select(g => new { VideoId = g.Key, Count = g.Count() })
				.ToListAsync(cancellationToken);

			foreach (var item in rawUnitCounts)
			{
				unreviewedUnitsByVideo[item.VideoId.Value] = item.Count;
			}
		}

		var needsAttentionVideos = queueRaw.Select(v => new CuratorVideoQueueItemDto(
			v.Id.Value,
			v.Title,
			v.Status.ToString(),
			v.SourceType.ToString(),
			v.ContentCategoryName,
			(int)Math.Round(v.Duration.TotalSeconds),
			unreviewedUnitsByVideo.GetValueOrDefault(v.Id.Value),
			v.CreatedAt,
			v.UpdatedAt)).ToList();

		return new CuratorVideosAnalyticsDto(
			totalVideos,
			readyCount,
			reviewingCount,
			pendingCount,
			processingCount,
			draftCount,
			rejectedCount,
			archivedCount,
			curatedCount,
			learnerUploadedCount,
			externalImportedCount,
			periodAddedCount,
			periodReadyCount,
			unreviewedTranscriptsCount,
			unreviewedLearningUnitsCount,
			lowConfidenceLearningUnitsCount,
			statusBreakdown,
			needsAttentionVideos);
	}
}
