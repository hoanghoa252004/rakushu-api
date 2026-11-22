using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Abstractions.Persistence.Queries;
using Rakushu.Application.Common.Time;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;

namespace Rakushu.Application.Usecases.Analytics.Learner.GetLearnerDashboard;

internal sealed class GetLearnerDashboardHandler
	: IRequestHandler<GetLearnerDashboardQuery, Result<LearnerDashboardDto>>
{
	private readonly ILearnerAnalyticsQueryService _analyticsQueryService;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;

	public GetLearnerDashboardHandler(
		ILearnerAnalyticsQueryService analyticsQueryService,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock)
	{
		_analyticsQueryService = analyticsQueryService;
		_currentUserContext = currentUserContext;
		_systemClock = systemClock;
	}

	public async Task<Result<LearnerDashboardDto>> Handle(
		GetLearnerDashboardQuery request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
		{
			return Result.Failure<LearnerDashboardDto>(UserErrors.NotFound);
		}

		var utcNow = _systemClock.UtcNow;
		var timeZone = AnalyticsTimeZoneHelper.ResolveTimeZone(request.TimeZone);
		int heatmapYear = request.HeatmapYear ?? AnalyticsTimeZoneHelper.GetLocalToday(utcNow, timeZone).Year;

		var dashboard = await _analyticsQueryService.GetLearnerDashboardAsync(
			userId,
			request.Days,
			request.ForecastDays,
			timeZone,
			heatmapYear,
			utcNow,
			cancellationToken);

		return Result.Success(dashboard);
	}
}
