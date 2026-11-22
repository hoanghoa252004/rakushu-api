using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Abstractions.Persistence.Queries;
using Rakushu.Application.Common.Time;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Analytics.Curator.GetCuratorDashboard;

internal sealed class GetCuratorDashboardHandler
	: IRequestHandler<GetCuratorDashboardQuery, Result<CuratorDashboardDto>>
{
	private readonly ICuratorAnalyticsQueryService _analyticsQueryService;
	private readonly ISystemClock _systemClock;

	public GetCuratorDashboardHandler(
		ICuratorAnalyticsQueryService analyticsQueryService,
		ISystemClock systemClock)
	{
		_analyticsQueryService = analyticsQueryService;
		_systemClock = systemClock;
	}

	public async Task<Result<CuratorDashboardDto>> Handle(
		GetCuratorDashboardQuery request,
		CancellationToken cancellationToken)
	{
		var utcNow = _systemClock.UtcNow;
		var timeZone = AnalyticsTimeZoneHelper.ResolveTimeZone(request.TimeZone);

		var dashboard = await _analyticsQueryService.GetCuratorDashboardAsync(
			request.Days,
			timeZone,
			utcNow,
			cancellationToken);

		return Result.Success(dashboard);
	}
}
