using Rakushu.Application.Abstractions.Persistence.Queries;
using Rakushu.Application.Usecases.Analytics.Learner;
using Rakushu.Domain.Entities.Identity.User;

namespace Rakushu.Persistence.Queries;

internal sealed partial class LearnerAnalyticsQueryService : ILearnerAnalyticsQueryService
{
	private const int MasteredIntervalDays = 21;

	private readonly RakushuDbContext _context;

	public LearnerAnalyticsQueryService(RakushuDbContext context)
	{
		_context = context;
	}

	public async Task<LearnerDashboardDto> GetLearnerDashboardAsync(
		UserId userId,
		int days,
		int forecastDays,
		TimeZoneInfo timeZone,
		int heatmapYear,
		DateTimeOffset utcNow,
		CancellationToken cancellationToken = default)
	{
		// RakushuDbContext is scoped and not thread-safe: execute sections sequentially.
		var coverageStats = await GetSrsCoverageStatisticsAsync(userId, cancellationToken);
		var srsAnalytics = await GetSrsAnalyticsSectionAsync(
			userId,
			forecastDays,
			timeZone,
			heatmapYear,
			utcNow,
			cancellationToken);

		var practicePerformance = await GetPracticePerformanceSectionAsync(
			userId,
			days,
			timeZone,
			utcNow,
			cancellationToken);

		var shadowingProgress = await GetDashboardShadowingProgressAsync(
			userId,
			days,
			utcNow,
			cancellationToken);

		var videoSummary = await GetVideoLearningSummarySectionAsync(
			userId,
			coverageStats,
			utcNow,
			cancellationToken);

		var retentionRate30d = await GetRecentRetentionRateAsync(
			userId,
			utcNow,
			cancellationToken);

		int totalSaved = coverageStats.TotalSaved;
		int masteredCount = coverageStats.TotalMastered;
		double masteryRatePercent = totalSaved > 0
			? Math.Round((double)masteredCount / totalSaved * 100.0, 1)
			: 0.0;

		var maturity = srsAnalytics.MaturityBreakdown;
		int activeWordsCount = maturity.NewCount + maturity.LearningCount + maturity.YoungCount + maturity.MasteredCount;
		double leechRatePercent = activeWordsCount > 0
			? Math.Round((double)maturity.LeechCount / activeWordsCount * 100.0, 1)
			: 0.0;

		double assessmentCompletionRate = shadowingProgress.TotalAttempts > 0
			? Math.Round((double)shadowingProgress.AssessedAttempts / shadowingProgress.TotalAttempts * 100.0, 1)
			: 0.0;

		string bestPerformingSkill = practicePerformance.ByQuestionType
			.Where(x => x.AnsweredCount > 0)
			.OrderByDescending(x => x.AccuracyRate)
			.ThenByDescending(x => x.AnsweredCount)
			.Select(x => x.QuestionTypeName)
			.FirstOrDefault() ?? string.Empty;

		string retentionStatusLabel = retentionRate30d >= 80.0 ? "Rất tốt" : "Khá";

		return new LearnerDashboardDto(
			days,
			utcNow,
			videoSummary,
			srsAnalytics,
			MapToKnowledgeCoverage(coverageStats),
			practicePerformance,
			shadowingProgress,
			masteryRatePercent,
			leechRatePercent,
			assessmentCompletionRate,
			bestPerformingSkill,
			retentionStatusLabel);
	}
}
