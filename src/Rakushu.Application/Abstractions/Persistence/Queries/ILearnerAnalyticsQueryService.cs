using Rakushu.Application.Usecases.Analytics.Learner;
using Rakushu.Application.Usecases.Analytics.Shadowing;
using Rakushu.Application.Usecases.Analytics.Shadowing.GetShadowingAttemptAssessments;
using Rakushu.Application.Usecases.Learning.Srs.Statistics;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Shadowing.ShadowingAttempt;

namespace Rakushu.Application.Abstractions.Persistence.Queries;

public interface ILearnerAnalyticsQueryService
{
	Task<SrsCoverageStatisticsDto> GetSrsCoverageStatisticsAsync(
		UserId userId,
		CancellationToken cancellationToken = default);

	Task<SrsHeatmapResponseDto> GetSrsHeatmapAsync(
		UserId userId,
		int year,
		TimeZoneInfo timeZone,
		CancellationToken cancellationToken = default);

	Task<LearnerDashboardDto> GetLearnerDashboardAsync(
		UserId userId,
		int days,
		int forecastDays,
		TimeZoneInfo timeZone,
		int heatmapYear,
		DateTimeOffset utcNow,
		CancellationToken cancellationToken = default);

	Task<QuestionTypeDetailResponseDto> GetQuizDiagnosticsAsync(
		UserId userId,
		int? days,
		string? questionTypeCode,
		DateTimeOffset utcNow,
		CancellationToken cancellationToken = default);

	Task<ShadowingAnalyticsDetailResponseDto> GetShadowingAnalyticsAsync(
		UserId userId,
		int? days,
		TimeZoneInfo timeZone,
		DateTimeOffset utcNow,
		CancellationToken cancellationToken = default);

	Task<ShadowingAttemptAssessmentContextModel?> GetAttemptAssessmentsContextAsync(
		ShadowingAttemptId attemptId,
		CancellationToken cancellationToken = default);
}
