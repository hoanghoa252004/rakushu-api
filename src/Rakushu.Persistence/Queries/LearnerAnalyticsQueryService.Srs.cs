using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Rakushu.Application.Common.Time;
using Rakushu.Application.Usecases.Analytics.Learner;
using Rakushu.Application.Usecases.Learning.Srs.Statistics;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;
using Rakushu.Persistence.Queries.Helpers;

namespace Rakushu.Persistence.Queries;

internal sealed partial class LearnerAnalyticsQueryService
{
	private static readonly (string Code, string Name)[] CanonicalLevels =
	[
		("N5", "Sơ cấp N5"),
		("N4", "Sơ trung cấp N4"),
		("N3", "Trung cấp N3"),
		("N2", "Trung cao cấp N2"),
		("N1", "Cao cấp N1"),
	];

	private static readonly (KnowledgeUnitType Type, string Label)[] CanonicalKnowledgeTypes =
	[
		(KnowledgeUnitType.Word, "Từ vựng"),
		(KnowledgeUnitType.Grammar, "Ngữ pháp"),
		(KnowledgeUnitType.Collocation, "Cụm từ"),
		(KnowledgeUnitType.Terminology, "Thuật ngữ"),
		(KnowledgeUnitType.ProperNoun, "Tên riêng"),
	];

	public async Task<SrsCoverageStatisticsDto> GetSrsCoverageStatisticsAsync(
		UserId userId,
		CancellationToken cancellationToken = default)
	{
		var rows = await _context.SavedItems
			.AsNoTracking()
			.Where(si => si.UserId == userId && si.IsActive)
			.Select(si => new
			{
				LevelCode = si.KnowledgeMeaning.KnowledgeUnit.ProficiencyLevel.Code,
				si.KnowledgeMeaning.KnowledgeUnit.KnowledgeType,
				LatestIntervalDays = si.Schedule != null
					? si.Schedule.Details
						.OrderByDescending(d => d.ReviewNumber)
						.Select(d => (int?)d.IntervalDays)
						.FirstOrDefault() ?? 0
					: 0,
			})
			.ToListAsync(cancellationToken);

		var grouped = rows
			.GroupBy(r => (
				Level: (r.LevelCode ?? string.Empty).Trim().ToUpperInvariant(),
				r.KnowledgeType))
			.ToDictionary(
				g => g.Key,
				g => (
					Saved: g.Count(),
					Mastered: g.Count(x => x.LatestIntervalDays >= MasteredIntervalDays)));

		var matrix = new List<CoverageMatrixCellDto>(CanonicalLevels.Length * CanonicalKnowledgeTypes.Length);
		foreach (var lvl in CanonicalLevels)
		{
			foreach (var kt in CanonicalKnowledgeTypes)
			{
				grouped.TryGetValue((lvl.Code, kt.Type), out var counts);
				int pct = counts.Saved > 0
					? (int)Math.Round((double)counts.Mastered / counts.Saved * 100.0)
					: 0;

				matrix.Add(new CoverageMatrixCellDto(
					lvl.Code,
					kt.Type.ToString(),
					counts.Saved,
					counts.Mastered,
					pct));
			}
		}

		var byLevel = CanonicalLevels.Select(lvl =>
		{
			var cells = matrix.Where(c => c.JlptLevel == lvl.Code).ToList();
			int saved = cells.Sum(c => c.TotalSaved);
			int mastered = cells.Sum(c => c.MasteredCount);
			double rate = saved > 0 ? Math.Round((double)mastered / saved * 100.0) : 0.0;
			return new SrsCoverageLevelDto(lvl.Code, saved, mastered, rate);
		}).ToList();

		var byType = CanonicalKnowledgeTypes.Select(kt =>
		{
			var typeKey = kt.Type.ToString();
			var cells = matrix.Where(c => c.KnowledgeType == typeKey).ToList();
			int saved = cells.Sum(c => c.TotalSaved);
			int mastered = cells.Sum(c => c.MasteredCount);
			double rate = saved > 0 ? Math.Round((double)mastered / saved * 100.0) : 0.0;
			return new SrsCoverageTypeDto(typeKey, kt.Label, saved, mastered, rate);
		}).ToList();

		int totalSaved = byLevel.Sum(l => l.TotalSaved);
		int totalMastered = byLevel.Sum(l => l.MasteredCount);
		double overallRate = totalSaved > 0 ? Math.Round((double)totalMastered / totalSaved * 100.0) : 0.0;

		return new SrsCoverageStatisticsDto(
			totalSaved,
			totalMastered,
			overallRate,
			matrix,
			byLevel,
			byType);
	}

	public async Task<SrsHeatmapResponseDto> GetSrsHeatmapAsync(
		UserId userId,
		int year,
		TimeZoneInfo timeZone,
		CancellationToken cancellationToken = default)
	{
		var (startUtc, endUtc) = AnalyticsTimeZoneHelper.GetUtcBoundsForYear(year, timeZone);

		var reviews = await _context.SrsScheduleDetails
			.AsNoTracking()
			.Where(d => d.ReviewedAt >= startUtc
				&& d.ReviewedAt < endUtc
				&& d.SrsSchedule.SavedItem.UserId == userId)
			.Select(d => new { d.ReviewedAt, d.Quality })
			.ToListAsync(cancellationToken);

		var points = reviews
			.GroupBy(r => AnalyticsTimeZoneHelper.ToLocalDate(r.ReviewedAt, timeZone))
			.OrderBy(g => g.Key)
			.Select(g => new DailyReviewHeatmapPointDto(
				g.Key.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
				g.Count(),
				g.Count(x => x.Quality >= 3)))
			.ToList();

		return new SrsHeatmapResponseDto(year, points);
	}

	private async Task<double> GetRecentRetentionRateAsync(
		UserId userId,
		DateTimeOffset utcNow,
		CancellationToken cancellationToken)
	{
		var thirtyDaysAgo = utcNow.AddDays(-30);
		var summary = await _context.SrsScheduleDetails
			.AsNoTracking()
			.Where(d => d.ReviewedAt >= thirtyDaysAgo
				&& d.SrsSchedule.SavedItem.UserId == userId
				&& d.SrsSchedule.SavedItem.IsActive)
			.GroupBy(_ => 1)
			.Select(g => new
			{
				TotalReviews = g.Count(),
				SuccessfulReviews = g.Count(d => d.Quality >= 3)
			})
			.FirstOrDefaultAsync(cancellationToken);

		if (summary is null || summary.TotalReviews == 0)
		{
			return 0.0;
		}

		return Math.Round((double)summary.SuccessfulReviews / summary.TotalReviews * 100.0, 1);
	}

	private async Task<LearnerSrsAnalyticsDto> GetSrsAnalyticsSectionAsync(
		UserId userId,
		int forecastDays,
		TimeZoneInfo timeZone,
		int heatmapYear,
		DateTimeOffset utcNow,
		CancellationToken cancellationToken)
	{
		var localToday = AnalyticsTimeZoneHelper.GetLocalToday(utcNow, timeZone);

		// 1. All distinct review dates (all-time) for accurate current & longest streaks.
		var allReviewTimestamps = await _context.SrsScheduleDetails
			.AsNoTracking()
			.Where(d => d.SrsSchedule.SavedItem.UserId == userId)
			.Select(d => d.ReviewedAt)
			.ToListAsync(cancellationToken);

		var distinctLocalDates = allReviewTimestamps
			.Select(ts => AnalyticsTimeZoneHelper.ToLocalDate(ts, timeZone))
			.Distinct()
			.ToList();

		var (currentStreak, longestStreak) = AnalyticsStreakCalculator.ComputeStreaks(distinctLocalDates, localToday);

		// 2. Maturity breakdown + upcoming due dates over active saved items.
		var savedSnapshots = await _context.SavedItems
			.AsNoTracking()
			.Where(si => si.UserId == userId && si.IsActive)
			.Select(si => new
			{
				DueAt = si.Schedule != null ? (DateTimeOffset?)si.Schedule.DueAt : null,
				Latest = si.Schedule != null
					? si.Schedule.Details
						.OrderByDescending(d => d.ReviewNumber)
						.Select(d => new { d.IntervalDays, d.EasinessFactor, d.Lapses })
						.FirstOrDefault()
					: null,
			})
			.ToListAsync(cancellationToken);

		int newCount = 0;
		int learningCount = 0;
		int youngCount = 0;
		int masteredCount = 0;
		int leechCount = 0;

		foreach (var item in savedSnapshots)
		{
			if (item.Latest is null)
			{
				newCount++;
				continue;
			}

			if (item.Latest.EasinessFactor <= 1.3001 || item.Latest.Lapses >= 8)
				leechCount++;

			if (item.Latest.IntervalDays >= MasteredIntervalDays)
				masteredCount++;
			else if (item.Latest.IntervalDays >= 7)
				youngCount++;
			else
				learningCount++;
		}

		var breakdown = new SrsMaturityBreakdownDto(
			newCount,
			learningCount,
			youngCount,
			masteredCount,
			leechCount);

		var forecast = BuildDueForecast(
			savedSnapshots.Where(s => s.DueAt.HasValue).Select(s => s.DueAt!.Value).ToList(),
			localToday,
			forecastDays,
			timeZone);

		var heatmap = await GetSrsHeatmapAsync(userId, heatmapYear, timeZone, cancellationToken);

		return new LearnerSrsAnalyticsDto(
			currentStreak,
			longestStreak,
			breakdown,
			heatmap.DailyReviewHeatmap,
			forecast);
	}

	private static IReadOnlyList<UpcomingDueForecastPointDto> BuildDueForecast(
		IReadOnlyList<DateTimeOffset> dueTimestamps,
		DateOnly localToday,
		int forecastDays,
		TimeZoneInfo timeZone)
	{
		var (_, todayEndUtc) = AnalyticsTimeZoneHelper.GetUtcBoundsForLocalDate(localToday, timeZone);
		var futureEndLocal = localToday.AddDays(forecastDays - 1);
		var (_, horizonEndUtc) = AnalyticsTimeZoneHelper.GetUtcBoundsForLocalDate(futureEndLocal, timeZone);

		int overdueAndTodayCount = 0;
		var countsByDate = new Dictionary<DateOnly, int>();

		foreach (var dueAt in dueTimestamps)
		{
			if (dueAt < todayEndUtc)
			{
				overdueAndTodayCount++;
			}
			else if (dueAt < horizonEndUtc)
			{
				var localDueDate = AnalyticsTimeZoneHelper.ToLocalDate(dueAt, timeZone);
				countsByDate[localDueDate] = countsByDate.GetValueOrDefault(localDueDate, 0) + 1;
			}
		}

		var points = new List<UpcomingDueForecastPointDto>(forecastDays);
		for (int i = 0; i < forecastDays; i++)
		{
			var targetDate = localToday.AddDays(i);
			int count = i == 0
				? overdueAndTodayCount
				: countsByDate.GetValueOrDefault(targetDate, 0);

			points.Add(new UpcomingDueForecastPointDto(
				targetDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
				count));
		}

		return points;
	}

	private static LearnerKnowledgeCoverageDto MapToKnowledgeCoverage(SrsCoverageStatisticsDto stats)
	{
		var levelNameMap = CanonicalLevels.ToDictionary(x => x.Code, x => x.Name, StringComparer.OrdinalIgnoreCase);

		var byLevel = stats.ByLevel
			.Select(l => new ProficiencyLevelCoverageDto(
				l.Level,
				levelNameMap.GetValueOrDefault(l.Level, l.Level),
				l.TotalSaved,
				l.MasteredCount,
				l.MasteryRate))
			.ToList();

		var byType = stats.ByType
			.Select(t => new KnowledgeTypeCoverageDto(
				t.Type,
				t.TotalSaved,
				t.MasteredCount))
			.ToList();

		var matrix = stats.Matrix
			.Select(m => new KnowledgeMatrixCellDto(
				m.JlptLevel,
				m.KnowledgeType,
				m.TotalSaved,
				m.MasteredCount))
			.ToList();

		return new LearnerKnowledgeCoverageDto(byLevel, byType, matrix);
	}
}
