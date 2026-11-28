using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Abstractions.Persistence.Queries;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Analytics.Admin.GetAdminMaterialsAnalytics;

internal sealed class GetAdminMaterialsAnalyticsHandler
	: IRequestHandler<GetAdminMaterialsAnalyticsQuery, Result<AdminMaterialsAnalyticsDto>>
{
	private readonly IAdminAnalyticsQueryService _adminAnalyticsQueryService;
	private readonly ISystemClock _systemClock;

	public GetAdminMaterialsAnalyticsHandler(
		IAdminAnalyticsQueryService adminAnalyticsQueryService,
		ISystemClock systemClock)
	{
		_adminAnalyticsQueryService = adminAnalyticsQueryService;
		_systemClock = systemClock;
	}

	public async Task<Result<AdminMaterialsAnalyticsDto>> Handle(
		GetAdminMaterialsAnalyticsQuery request,
		CancellationToken cancellationToken)
	{
		var analytics = await _adminAnalyticsQueryService.GetAdminMaterialsAnalyticsAsync(
			request.DaysWindow,
			_systemClock.UtcNow,
			cancellationToken);

		return Result.Success(analytics);
	}
}
