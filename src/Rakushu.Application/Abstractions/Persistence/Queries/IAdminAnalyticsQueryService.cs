using Rakushu.Application.Usecases.Analytics.Admin;

namespace Rakushu.Application.Abstractions.Persistence.Queries;

public interface IAdminAnalyticsQueryService
{
	Task<AdminMaterialsAnalyticsDto> GetAdminMaterialsAnalyticsAsync(
		int daysWindow,
		DateTimeOffset utcNow,
		CancellationToken cancellationToken = default);
}
