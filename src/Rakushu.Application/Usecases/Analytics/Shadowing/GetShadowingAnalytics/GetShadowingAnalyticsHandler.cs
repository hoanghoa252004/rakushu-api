using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Abstractions.Persistence.Queries;
using Rakushu.Application.Common.Time;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;

namespace Rakushu.Application.Usecases.Analytics.Shadowing.GetShadowingAnalytics;

internal sealed class GetShadowingAnalyticsHandler
	: IRequestHandler<GetShadowingAnalyticsQuery, Result<ShadowingAnalyticsDetailResponseDto>>
{
	private readonly ILearnerAnalyticsQueryService _analyticsQueryService;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;

	public GetShadowingAnalyticsHandler(
		ILearnerAnalyticsQueryService analyticsQueryService,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock)
	{
		_analyticsQueryService = analyticsQueryService;
		_currentUserContext = currentUserContext;
		_systemClock = systemClock;
	}

	public async Task<Result<ShadowingAnalyticsDetailResponseDto>> Handle(
		GetShadowingAnalyticsQuery request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
		{
			return Result.Failure<ShadowingAnalyticsDetailResponseDto>(UserErrors.NotFound);
		}

		int? days = request.Period?.Trim().ToLowerInvariant() switch
		{
			"7d" => 7,
			"90d" => 90,
			"all" => null,
			_ => 30,
		};

		var timeZone = AnalyticsTimeZoneHelper.ResolveTimeZone(request.TimeZone);

		var analytics = await _analyticsQueryService.GetShadowingAnalyticsAsync(
			userId,
			days,
			timeZone,
			_systemClock.UtcNow,
			cancellationToken);

		return Result.Success(analytics);
	}
}
