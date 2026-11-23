using Rakushu.Application.Usecases.Analytics.Curator;

namespace Rakushu.Application.Abstractions.Persistence.Queries;

public interface ICuratorAnalyticsQueryService
{
	Task<CuratorDashboardDto> GetCuratorDashboardAsync(
		int days,
		TimeZoneInfo timeZone,
		DateTimeOffset utcNow,
		CancellationToken cancellationToken = default);

	Task<CuratorSummaryDto> GetCuratorSummaryAsync(
		DateTimeOffset utcNow,
		CancellationToken cancellationToken = default);

	Task<CuratorHighErrorQuestionsResponseDto> GetHighErrorQuestionsAsync(
		int days,
		TimeZoneInfo timeZone,
		DateTimeOffset utcNow,
		string? level,
		string? questionTypeCode,
		string? search,
		int minAnswers,
		string? sortBy,
		int page,
		int pageSize,
		CancellationToken cancellationToken = default);
}
