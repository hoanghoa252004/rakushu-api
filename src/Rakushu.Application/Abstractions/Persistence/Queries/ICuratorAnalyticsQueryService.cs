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
}
