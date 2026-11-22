using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Abstractions.Persistence.Queries;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;

namespace Rakushu.Application.Usecases.Analytics.Learner.GetQuizDiagnostics;

internal sealed class GetQuizDiagnosticsHandler
	: IRequestHandler<GetQuizDiagnosticsQuery, Result<QuestionTypeDetailResponseDto>>
{
	private readonly ILearnerAnalyticsQueryService _analyticsQueryService;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;

	public GetQuizDiagnosticsHandler(
		ILearnerAnalyticsQueryService analyticsQueryService,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock)
	{
		_analyticsQueryService = analyticsQueryService;
		_currentUserContext = currentUserContext;
		_systemClock = systemClock;
	}

	public async Task<Result<QuestionTypeDetailResponseDto>> Handle(
		GetQuizDiagnosticsQuery request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
		{
			return Result.Failure<QuestionTypeDetailResponseDto>(UserErrors.NotFound);
		}

		var diagnostics = await _analyticsQueryService.GetQuizDiagnosticsAsync(
			userId,
			request.Days,
			request.QuestionTypeCode,
			_systemClock.UtcNow,
			cancellationToken);

		return Result.Success(diagnostics);
	}
}
