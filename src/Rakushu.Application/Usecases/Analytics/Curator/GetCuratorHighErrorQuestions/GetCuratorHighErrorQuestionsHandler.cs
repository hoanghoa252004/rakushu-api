using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Abstractions.Persistence.Queries;
using Rakushu.Application.Common.Time;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Analytics.Curator.GetCuratorHighErrorQuestions;

internal sealed class GetCuratorHighErrorQuestionsHandler
	: IRequestHandler<GetCuratorHighErrorQuestionsQuery, Result<CuratorHighErrorQuestionsResponseDto>>
{
	private readonly ICuratorAnalyticsQueryService _analyticsQueryService;
	private readonly ISystemClock _systemClock;

	public GetCuratorHighErrorQuestionsHandler(
		ICuratorAnalyticsQueryService analyticsQueryService,
		ISystemClock systemClock)
	{
		_analyticsQueryService = analyticsQueryService;
		_systemClock = systemClock;
	}

	public async Task<Result<CuratorHighErrorQuestionsResponseDto>> Handle(
		GetCuratorHighErrorQuestionsQuery request,
		CancellationToken cancellationToken)
	{
		var utcNow = _systemClock.UtcNow;
		var timeZone = AnalyticsTimeZoneHelper.ResolveTimeZone(request.TimeZone);

		var response = await _analyticsQueryService.GetHighErrorQuestionsAsync(
			request.Days,
			timeZone,
			utcNow,
			request.Level,
			request.QuestionTypeCode,
			request.Search,
			request.MinAnswers,
			request.SortBy,
			request.Page,
			request.PageSize,
			cancellationToken);

		return Result.Success(response);
	}
}
