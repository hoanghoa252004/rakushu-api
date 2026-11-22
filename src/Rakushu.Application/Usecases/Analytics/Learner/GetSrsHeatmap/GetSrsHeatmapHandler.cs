using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Abstractions.Persistence.Queries;
using Rakushu.Application.Common.Time;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;

namespace Rakushu.Application.Usecases.Analytics.Learner.GetSrsHeatmap;

internal sealed class GetSrsHeatmapHandler
	: IRequestHandler<GetSrsHeatmapQuery, Result<SrsHeatmapResponseDto>>
{
	private readonly ILearnerAnalyticsQueryService _analyticsQueryService;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;

	public GetSrsHeatmapHandler(
		ILearnerAnalyticsQueryService analyticsQueryService,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock)
	{
		_analyticsQueryService = analyticsQueryService;
		_currentUserContext = currentUserContext;
		_systemClock = systemClock;
	}

	public async Task<Result<SrsHeatmapResponseDto>> Handle(
		GetSrsHeatmapQuery request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
		{
			return Result.Failure<SrsHeatmapResponseDto>(UserErrors.NotFound);
		}

		var timeZone = AnalyticsTimeZoneHelper.ResolveTimeZone(request.TimeZone);
		int year = request.Year ?? AnalyticsTimeZoneHelper.GetLocalToday(_systemClock.UtcNow, timeZone).Year;

		var heatmap = await _analyticsQueryService.GetSrsHeatmapAsync(
			userId,
			year,
			timeZone,
			cancellationToken);

		return Result.Success(heatmap);
	}
}
