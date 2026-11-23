using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Abstractions.Persistence.Queries;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Analytics.Curator.GetCuratorSummary;

internal sealed class GetCuratorSummaryHandler
	: IRequestHandler<GetCuratorSummaryQuery, Result<CuratorSummaryDto>>
{
	private readonly ICuratorAnalyticsQueryService _analyticsQueryService;
	private readonly ISystemClock _systemClock;

	public GetCuratorSummaryHandler(
		ICuratorAnalyticsQueryService analyticsQueryService,
		ISystemClock systemClock)
	{
		_analyticsQueryService = analyticsQueryService;
		_systemClock = systemClock;
	}

	public async Task<Result<CuratorSummaryDto>> Handle(
		GetCuratorSummaryQuery request,
		CancellationToken cancellationToken)
	{
		var utcNow = _systemClock.UtcNow;
		var summary = await _analyticsQueryService.GetCuratorSummaryAsync(utcNow, cancellationToken);
		return Result.Success(summary);
	}
}
