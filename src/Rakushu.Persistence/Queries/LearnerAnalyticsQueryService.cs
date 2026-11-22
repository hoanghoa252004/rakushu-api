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

		return new LearnerDashboardDto(
			days,
			utcNow,
			videoSummary,
			srsAnalytics,
			MapToKnowledgeCoverage(coverageStats),
			practicePerformance,
			shadowingProgress);
	}
}
