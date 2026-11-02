using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.SavedItem;

namespace Rakushu.Application.Usecases.Learning.Srs.SavedItem.GetSrsDashboardMetrics;

internal sealed class GetSrsDashboardMetricsHandler : IRequestHandler<GetSrsDashboardMetricsQuery, Result<SrsDashboardMetricsDto>>
{
	private readonly ISavedItemRepository _savedItemRepository;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;

	public GetSrsDashboardMetricsHandler(
		ISavedItemRepository savedItemRepository,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock)
	{
		_savedItemRepository = savedItemRepository;
		_currentUserContext = currentUserContext;
		_systemClock = systemClock;
	}

	public async Task<Result<SrsDashboardMetricsDto>> Handle(
		GetSrsDashboardMetricsQuery request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
		{
			return Result.Failure<SrsDashboardMetricsDto>(UserErrors.NotFound);
		}

		var now = _systemClock.UtcNow;
		var metrics = await _savedItemRepository.GetDashboardMetricsAsync(userId, now, cancellationToken);

		var dto = new SrsDashboardMetricsDto(
			metrics.TotalDueCount,
			metrics.DueTodayCount,
			metrics.OverdueCount,
			metrics.TotalActiveWords,
			metrics.TrueRetentionRate,
			metrics.SuccessfulReviewsLast30Days,
			metrics.TotalReviewsLast30Days,
			metrics.LeechCount,
			metrics.MasteryRate,
			metrics.MasteredWordsCount);

		return Result.Success(dto);
	}
}
