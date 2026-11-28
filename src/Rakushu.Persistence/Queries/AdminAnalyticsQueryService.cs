using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Rakushu.Application.Abstractions.Persistence.Queries;
using Rakushu.Application.Usecases.Analytics.Admin;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Material.Video.MediaAsset;
using Rakushu.Domain.Entities.Processing.ProcessingJobLog;

namespace Rakushu.Persistence.Queries;

internal sealed class AdminAnalyticsQueryService : IAdminAnalyticsQueryService
{
	private const double BytesPerMegabyte = 1024.0 * 1024.0;
	private const double EstimatedVideoBytesPerSecond = 1.2 * 1024.0 * 1024.0 / 60.0;
	private const double EstimatedAudioBytesPerSecond = 192.0 * 1024.0 / 60.0;
	private const long EstimatedThumbnailBytesPerVideo = 150L * 1024L;

	private static readonly VideoStatus[] CanonicalStatuses =
	[
		VideoStatus.Ready,
		VideoStatus.Reviewing,
		VideoStatus.Pending,
		VideoStatus.Processing,
		VideoStatus.Draft,
		VideoStatus.Rejected,
		VideoStatus.Archived
	];

	private readonly RakushuDbContext _context;

	public AdminAnalyticsQueryService(RakushuDbContext context)
	{
		_context = context;
	}

	public async Task<AdminMaterialsAnalyticsDto> GetAdminMaterialsAnalyticsAsync(
		int daysWindow,
		DateTimeOffset utcNow,
		CancellationToken cancellationToken = default)
	{
		// 1. Video Status breakdown (SQL GROUP BY).
		var rawStatusCounts = await _context.Videos
			.AsNoTracking()
			.GroupBy(v => v.Status)
			.Select(g => new { Status = g.Key, Count = g.Count() })
			.ToListAsync(cancellationToken);

		var statusMap = rawStatusCounts.ToDictionary(x => x.Status, x => x.Count);
		int totalVideos = rawStatusCounts.Sum(x => x.Count);
		int activeVideoCount = statusMap.GetValueOrDefault(VideoStatus.Ready);

		var statusBreakdown = new List<AdminMaterialsStatusMetricDto>(CanonicalStatuses.Length + 2);
		foreach (var status in CanonicalStatuses)
		{
			int count = statusMap.GetValueOrDefault(status);
			statusBreakdown.Add(new AdminMaterialsStatusMetricDto(
				status.ToString(),
				count,
				CalculatePercentage(count, totalVideos)));
		}

		// Include extra pipeline statuses (Importing, Moderating) if present in DB so counts reconcile with TotalVideos.
		foreach (var extra in rawStatusCounts.Where(x => !CanonicalStatuses.Contains(x.Status) && x.Count > 0))
		{
			statusBreakdown.Add(new AdminMaterialsStatusMetricDto(
				extra.Status.ToString(),
				extra.Count,
				CalculatePercentage(extra.Count, totalVideos)));
		}

		// 2. Source breakdown (SQL GROUP BY).
		var rawSourceCounts = await _context.Videos
			.AsNoTracking()
			.GroupBy(v => v.SourceType)
			.Select(g => new { SourceType = g.Key, Count = g.Count() })
			.ToListAsync(cancellationToken);

		var sourceMap = rawSourceCounts.ToDictionary(x => x.SourceType, x => x.Count);
		int curatedCount = sourceMap.GetValueOrDefault(VideoSource.Curated);
		int learnerUploadedCount = sourceMap.GetValueOrDefault(VideoSource.UploadedByLearner);
		int externalImportedCount = sourceMap.GetValueOrDefault(VideoSource.ImportedFromExternal);

		var sourceBreakdown = new AdminMaterialsSourceBreakdownDto(
			curatedCount,
			CalculatePercentage(curatedCount, totalVideos),
			learnerUploadedCount,
			CalculatePercentage(learnerUploadedCount, totalVideos),
			externalImportedCount,
			CalculatePercentage(externalImportedCount, totalVideos));

		// 3. Recent added count & total duration (lightweight scalar queries).
		var cutoffUtc = utcNow.AddDays(-daysWindow);
		int recentAddedCount = await _context.Videos
			.AsNoTracking()
			.CountAsync(v => v.CreatedAt >= cutoffUtc, cancellationToken);

		var durations = await _context.Videos
			.AsNoTracking()
			.Select(v => v.Duration)
			.ToListAsync(cancellationToken);

		long totalDurationSeconds = durations.Count > 0
			? (long)Math.Round(durations.Sum(d => Math.Max(0.0, d.TotalSeconds)))
			: 0L;
		int totalHours = (int)(totalDurationSeconds / 3600L);
		int totalMinutes = (int)((totalDurationSeconds % 3600L) / 60L);
		int avgDurationSeconds = totalVideos > 0
			? (int)Math.Round((double)totalDurationSeconds / totalVideos)
			: 0;
		string formattedAvgDuration = FormatDuration(avgDurationSeconds);

		// 4. Top 5 categories (SQL GROUP BY + Take(5)).
		var topCategoryRows = await _context.Videos
			.AsNoTracking()
			.GroupBy(v => new
			{
				v.ContentCategoryId,
				CategoryName = v.ContentCategory.Name,
				JapaneseName = v.ContentCategory.JapaneseName
			})
			.Select(g => new
			{
				g.Key.ContentCategoryId,
				g.Key.CategoryName,
				g.Key.JapaneseName,
				VideoCount = g.Count()
			})
			.OrderByDescending(x => x.VideoCount)
			.ThenBy(x => x.CategoryName)
			.Take(5)
			.ToListAsync(cancellationToken);

		var topCategories = topCategoryRows
			.Select(c => new AdminMaterialsCategoryMetricDto(
				c.ContentCategoryId.Value,
				c.CategoryName,
				c.JapaneseName,
				c.VideoCount,
				CalculatePercentage(c.VideoCount, totalVideos)))
			.ToList();

		// 5. Storage breakdown from MediaAssets (SQL GROUP BY AssetType).
		var rawAssetStorage = await _context.MediaAssets
			.AsNoTracking()
			.GroupBy(a => a.AssetType)
			.Select(g => new
			{
				AssetType = g.Key,
				TotalBytes = g.Sum(a => a.FileSizeBytes),
				FileCount = g.Count()
			})
			.ToListAsync(cancellationToken);

		long videoBytes = 0;
		int videoFileCount = 0;
		long audioBytes = 0;
		int audioFileCount = 0;
		long thumbnailBytes = 0;
		int thumbnailFileCount = 0;

		foreach (var row in rawAssetStorage)
		{
			switch (row.AssetType)
			{
				case MediaAssetType.Video:
				case MediaAssetType.Preview:
					videoBytes += Math.Max(0L, row.TotalBytes);
					videoFileCount += row.FileCount;
					break;
				case MediaAssetType.Audio:
					audioBytes += Math.Max(0L, row.TotalBytes);
					audioFileCount += row.FileCount;
					break;
				case MediaAssetType.Thumbnail:
					thumbnailBytes += Math.Max(0L, row.TotalBytes);
					thumbnailFileCount += row.FileCount;
					break;
			}
		}

		long totalStorageBytes = videoBytes + audioBytes + thumbnailBytes;

		// Fallback to duration-based estimation only if no real MediaAsset file sizes exist yet.
		if (totalStorageBytes == 0 && totalVideos > 0)
		{
			videoBytes = (long)Math.Round(totalDurationSeconds * EstimatedVideoBytesPerSecond);
			audioBytes = (long)Math.Round(totalDurationSeconds * EstimatedAudioBytesPerSecond);
			thumbnailBytes = totalVideos * EstimatedThumbnailBytesPerVideo;
			totalStorageBytes = videoBytes + audioBytes + thumbnailBytes;
			videoFileCount = totalVideos;
			audioFileCount = totalVideos;
			thumbnailFileCount = totalVideos;
		}

		var storageBreakdown = new AdminMaterialsStorageBreakdownDto(
			Math.Round(videoBytes / BytesPerMegabyte, 1),
			CalculatePercentage(videoBytes, totalStorageBytes),
			videoFileCount,
			Math.Round(audioBytes / BytesPerMegabyte, 1),
			CalculatePercentage(audioBytes, totalStorageBytes),
			audioFileCount,
			Math.Round(thumbnailBytes / BytesPerMegabyte, 1),
			CalculatePercentage(thumbnailBytes, totalStorageBytes),
			thumbnailFileCount);

		// 6. AI Video processing job logs (TargetType == Video).
		var videoJobLogs = await _context.ProcessingJobLogs
			.AsNoTracking()
			.Where(j => j.TargetType == ProcessingJobTargetType.Video)
			.Select(j => new
			{
				j.IsSuccess,
				j.StartAt,
				j.EndAt
			})
			.ToListAsync(cancellationToken);

		int aiTotalJobs = videoJobLogs.Count;
		int aiSuccessfulJobs = videoJobLogs.Count(j => j.IsSuccess);
		bool hasJobLogs = aiTotalJobs > 0;
		double aiSuccessRate = CalculatePercentage(aiSuccessfulJobs, aiTotalJobs);

		var finishedDurations = videoJobLogs
			.Where(j => j.EndAt.HasValue && j.EndAt.Value >= j.StartAt)
			.Select(j => (j.EndAt!.Value - j.StartAt).TotalSeconds)
			.ToList();

		int aiAvgProcessingSeconds = finishedDurations.Count > 0
			? (int)Math.Round(finishedDurations.Average())
			: 0;

		var kpis = new AdminMaterialsKpiDto(
			totalVideos,
			activeVideoCount,
			totalDurationSeconds,
			totalHours,
			totalMinutes,
			recentAddedCount,
			daysWindow,
			avgDurationSeconds,
			formattedAvgDuration,
			totalStorageBytes,
			FormatBytes(totalStorageBytes),
			audioFileCount,
			videoFileCount + thumbnailFileCount,
			aiAvgProcessingSeconds,
			aiSuccessRate,
			aiTotalJobs,
			aiSuccessfulJobs,
			hasJobLogs);

		return new AdminMaterialsAnalyticsDto(
			kpis,
			sourceBreakdown,
			topCategories,
			storageBreakdown,
			statusBreakdown);
	}

	private static double CalculatePercentage(long count, long total)
	{
		return total > 0 ? Math.Round((double)count / total * 100.0, 1) : 0.0;
	}

	private static string FormatDuration(int totalSeconds)
	{
		if (totalSeconds <= 0)
		{
			return "0s";
		}

		int hours = totalSeconds / 3600;
		int minutes = (totalSeconds % 3600) / 60;
		int seconds = totalSeconds % 60;

		if (hours > 0)
		{
			return $"{hours}h {minutes}m {seconds}s";
		}

		if (minutes > 0)
		{
			return $"{minutes}m {seconds}s";
		}

		return $"{seconds}s";
	}

	private static string FormatBytes(long bytes)
	{
		if (bytes <= 0)
		{
			return "0 B";
		}

		string[] units = ["B", "KB", "MB", "GB", "TB"];
		double value = bytes;
		int unitIndex = 0;

		while (value >= 1024.0 && unitIndex < units.Length - 1)
		{
			value /= 1024.0;
			unitIndex++;
		}

		return unitIndex == 0
			? $"{bytes} B"
			: $"{value.ToString("0.0", CultureInfo.InvariantCulture)} {units[unitIndex]}";
	}
}
