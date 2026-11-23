using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Persistence.Queries;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;

namespace Rakushu.Application.Usecases.Learning.Srs.Statistics.GetSrsCoverageStatistics;

internal sealed class GetSrsCoverageStatisticsHandler
	: IRequestHandler<GetSrsCoverageStatisticsQuery, Result<SrsCoverageStatisticsDto>>
{
	private readonly ILearnerAnalyticsQueryService _analyticsQueryService;
	private readonly ICurrentUserContext _currentUserContext;

	public GetSrsCoverageStatisticsHandler(
		ILearnerAnalyticsQueryService analyticsQueryService,
		ICurrentUserContext currentUserContext)
	{
		_analyticsQueryService = analyticsQueryService;
		_currentUserContext = currentUserContext;
	}

	public async Task<Result<SrsCoverageStatisticsDto>> Handle(
		GetSrsCoverageStatisticsQuery request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
		{
			return Result.Failure<SrsCoverageStatisticsDto>(UserErrors.NotFound);
		}

		var stats = await _analyticsQueryService.GetSrsCoverageStatisticsAsync(userId, cancellationToken);
		return Result.Success(stats);
	}
}
