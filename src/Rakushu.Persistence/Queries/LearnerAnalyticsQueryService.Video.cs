using Microsoft.EntityFrameworkCore;
using Rakushu.Application.Usecases.Analytics.Learner;
using Rakushu.Application.Usecases.Learning.Srs.Statistics;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Persistence.Queries;

internal sealed partial class LearnerAnalyticsQueryService
{
	private async Task<F1VideoLearningSummaryDto> GetVideoLearningSummarySectionAsync(
		UserId userId,
		SrsCoverageStatisticsDto coverageStats,
		DateTimeOffset utcNow,
		CancellationToken cancellationToken)
	{
		// 1. Watch progress videos + last study timestamp.
		var watchedProgress = await _context.LearnerVideoProgresses
			.AsNoTracking()
			.Where(p => p.UserId == userId)
			.Select(p => new { p.VideoId, p.LastWatchedAt })
			.ToListAsync(cancellationToken);

		DateTimeOffset? lastStudiedAt = watchedProgress.Count > 0
			? watchedProgress.Max(p => p.LastWatchedAt)
			: null;

		// 2. Learner-uploaded personal videos.
		var uploadedVideos = await _context.Videos
			.AsNoTracking()
			.Where(v => v.LearnerId == userId && v.Status != VideoStatus.Archived)
			.Select(v => new { v.Status, v.Duration })
			.ToListAsync(cancellationToken);

		int uploadedVideosCount = uploadedVideos.Count;
		int uploadedTotalDurationSeconds = (int)Math.Round(uploadedVideos.Sum(v => v.Duration.TotalSeconds));
		int aiReadyCount = uploadedVideos.Count(v => v.Status == VideoStatus.Ready);
		int aiProcessingCount = uploadedVideos.Count(v =>
			v.Status == VideoStatus.Pending ||
			v.Status == VideoStatus.Processing ||
			v.Status == VideoStatus.Reviewing);

		// 3. Learner's active saved items (for weekly growth + video provenance + sample words).
		var sevenDaysAgo = utcNow.AddDays(-7);
		var fourteenDaysAgo = utcNow.AddDays(-14);

		var activeSavedItems = await _context.SavedItems
			.AsNoTracking()
			.Where(si => si.UserId == userId && si.IsActive)
			.Select(si => new
			{
				SavedItemId = si.Id.Value,
				MeaningId = si.KnowledgeMeaningId.Value,
				si.SavedAt,
				si.KnowledgeMeaning.KnowledgeUnit.Expression,
				si.KnowledgeMeaning.KnowledgeUnit.Reading,
				si.KnowledgeMeaning.Meaning,
			})
			.ToListAsync(cancellationToken);

		int totalSavedWords = activeSavedItems.Count;
		int weeklyHarvestCount = activeSavedItems.Count(s => s.SavedAt >= sevenDaysAgo);
		int prevWeekHarvestCount = activeSavedItems.Count(s => s.SavedAt >= fourteenDaysAgo && s.SavedAt < sevenDaysAgo);
		int weeklyGrowthCount = weeklyHarvestCount - prevWeekHarvestCount;
		int? weeklyGrowthPercentage = prevWeekHarvestCount > 0
			? (int)Math.Round((double)weeklyGrowthCount / prevWeekHarvestCount * 100.0)
			: weeklyHarvestCount > 0
				? 100
				: 0;

		var savedByMeaningId = activeSavedItems
			.GroupBy(s => s.MeaningId)
			.ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.SavedAt).First());

		// 4. Encounter provenance rows linking meanings & videos the learner studied.
		var encounters = await _context.LearnerEncounteredLearningUnits
			.AsNoTracking()
			.Where(e => e.UserId == userId && e.Video.Status != VideoStatus.Archived)
			.Select(e => new
			{
				VideoId = e.VideoId.Value,
				VideoTitle = e.Video.Title,
				CategoryId = e.Video.ContentCategoryId.Value,
				CategoryName = e.Video.ContentCategory.Name,
				CategorySlug = e.Video.ContentCategory.Slug,
				CategoryThemeColor = e.Video.ContentCategory.ThemeColor,
				MeaningId = e.KnowledgeMeaningId.Value,
			})
			.ToListAsync(cancellationToken);

		var exploredVideoIds = watchedProgress.Select(p => p.VideoId.Value).ToHashSet();
		foreach (var e in encounters)
		{
			exploredVideoIds.Add(e.VideoId);
		}

		int exploredVideosCount = exploredVideoIds.Count;
		var savedFromEncounters = encounters.Where(e => savedByMeaningId.ContainsKey(e.MeaningId)).ToList();

		// Fallback mapping via LearningUnits for saved meanings that predate encounter tracking.
		var unmatchedMeaningIds = savedByMeaningId.Keys
			.Except(savedFromEncounters.Select(e => e.MeaningId))
			.Select(KnowledgeMeaningId.From)
			.ToList();

		var fallbackLinks = unmatchedMeaningIds.Count > 0
			? await _context.LearningUnits
				.AsNoTracking()
				.Where(lu => unmatchedMeaningIds.Contains(lu.KnowledgeMeaningId)
					&& lu.TranscriptSegment.Transcript.Video.Status != VideoStatus.Archived)
				.Select(lu => new
				{
					VideoId = lu.TranscriptSegment.Transcript.VideoId.Value,
					VideoTitle = lu.TranscriptSegment.Transcript.Video.Title,
					CategoryId = lu.TranscriptSegment.Transcript.Video.ContentCategoryId.Value,
					CategoryName = lu.TranscriptSegment.Transcript.Video.ContentCategory.Name,
					CategorySlug = lu.TranscriptSegment.Transcript.Video.ContentCategory.Slug,
					CategoryThemeColor = lu.TranscriptSegment.Transcript.Video.ContentCategory.ThemeColor,
					MeaningId = lu.KnowledgeMeaningId.Value,
				})
				.ToListAsync(cancellationToken)
			: [];

		// Prefer real encounter records; supplement with LearningUnit links only for remaining meanings.
		var combinedLinks = savedFromEncounters.Concat(fallbackLinks).ToList();

		// Primary category per unique saved meaning (earliest/most encountered video category wins; avoids double counting).
		var meaningPrimaryCategory = combinedLinks
			.GroupBy(l => l.MeaningId)
			.Select(g =>
			{
				var topCat = g
					.GroupBy(x => new { x.CategoryId, x.CategoryName, x.CategorySlug, x.CategoryThemeColor })
					.OrderByDescending(cg => cg.Count())
					.Select(cg => cg.Key)
					.First();
				return (MeaningId: g.Key, Category: topCat);
			})
			.ToList();

		int attributedSavedCount = meaningPrimaryCategory.Count;
		int topicDenominator = Math.Max(1, attributedSavedCount);

		var topicDistribution = meaningPrimaryCategory
			.GroupBy(x => x.Category)
			.OrderByDescending(g => g.Count())
			.ThenBy(g => g.Key.CategoryName, StringComparer.Ordinal)
			.Select(g =>
			{
				int cnt = g.Count();
				int pct = (int)Math.Round((double)cnt / topicDenominator * 100.0);
				return new VideoTopicDistributionItemDto(
					g.Key.CategoryId,
					g.Key.CategoryName,
					cnt,
					pct,
					g.Key.CategoryThemeColor,
					g.Key.CategorySlug);
			})
			.ToList();

		var topCategory = topicDistribution.FirstOrDefault();

		// Top harvested videos ranked by distinct saved meanings encountered/present in each video.
		// Prefer videos the learner actually watched/encountered first.
		var topVideos = combinedLinks
			.GroupBy(l => new { l.VideoId, l.VideoTitle })
			.Select(g =>
			{
				bool isExploredByLearner = exploredVideoIds.Contains(g.Key.VideoId);
				var distinctMeanings = g.Select(x => x.MeaningId).Distinct().ToList();
				var samples = distinctMeanings
					.Take(3)
					.Select(mid => savedByMeaningId[mid])
					.Select(s => new VideoLearningSampleWordDto(s.Expression, s.Reading, s.Meaning))
					.ToList();

				return new
				{
					g.Key.VideoId,
					g.Key.VideoTitle,
					IsExplored = isExploredByLearner,
					SavedCount = distinctMeanings.Count,
					Samples = samples,
				};
			})
			.OrderByDescending(v => v.IsExplored)
			.ThenByDescending(v => v.SavedCount)
			.ThenBy(v => v.VideoTitle, StringComparer.Ordinal)
			.Take(4)
			.Select(v => new HarvestedVideoSummaryDto(
				v.VideoId,
				v.VideoTitle,
				v.SavedCount,
				v.Samples))
			.ToList();

		double avgWordsPerVideo = exploredVideosCount > 0
			? Math.Round((double)attributedSavedCount / exploredVideosCount, 1)
			: 0.0;

		int totalForJlptPct = Math.Max(1, coverageStats.TotalSaved);
		var jlptDistribution = coverageStats.ByLevel
			.Select(l => new JlptDistributionItemDto(
				l.Level,
				l.TotalSaved,
				coverageStats.TotalSaved > 0 ? (int)Math.Round((double)l.TotalSaved / totalForJlptPct * 100.0) : 0))
			.ToList();

		return new F1VideoLearningSummaryDto(
			exploredVideosCount,
			avgWordsPerVideo,
			weeklyHarvestCount,
			weeklyGrowthCount,
			weeklyGrowthPercentage,
			lastStudiedAt,
			uploadedVideosCount,
			uploadedTotalDurationSeconds,
			aiReadyCount,
			aiProcessingCount,
			totalSavedWords,
			topicDistribution.Count,
			topCategory?.Name ?? string.Empty,
			topCategory?.Percentage ?? 0,
			topicDistribution.Take(4).ToList(),
			jlptDistribution,
			topVideos);
	}
}
