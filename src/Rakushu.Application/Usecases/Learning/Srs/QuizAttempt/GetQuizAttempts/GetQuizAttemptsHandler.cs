using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.QuizAttempt;

namespace Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.GetQuizAttempts;

internal sealed class GetQuizAttemptsHandler : IRequestHandler<GetQuizAttemptsQuery, Result<PaginatedList<QuizAttemptSummaryDto>>>
{
	private readonly IQuizAttemptRepository _quizAttemptRepository;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;

	public GetQuizAttemptsHandler(
		IQuizAttemptRepository quizAttemptRepository,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock)
	{
		_quizAttemptRepository = quizAttemptRepository;
		_currentUserContext = currentUserContext;
		_systemClock = systemClock;
	}

	public async Task<Result<PaginatedList<QuizAttemptSummaryDto>>> Handle(
		GetQuizAttemptsQuery request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
			return Result.Failure<PaginatedList<QuizAttemptSummaryDto>>(UserErrors.NotFound);

		QuizAttemptStatus? statusFilter = null;
		if (!string.IsNullOrWhiteSpace(request.Status) &&
			Enum.TryParse<QuizAttemptStatus>(request.Status, true, out var parsedStatus))
		{
			statusFilter = parsedStatus;
		}

		var (attempts, totalCount) = await _quizAttemptRepository.GetAttemptsByUserAsync(
			userId,
			statusFilter,
			request.PageNumber,
			request.PageSize,
			cancellationToken);

		var now = _systemClock.UtcNow;

		var dtoList = attempts.Select(a =>
		{
			var currentStatus = a.Status;
			if (a.IsExpired(now))
			{
				currentStatus = QuizAttemptStatus.Expired;
			}

			bool isInProgress = currentStatus == QuizAttemptStatus.InProgress;

			return new QuizAttemptSummaryDto(
				a.Id.Value,
				a.Mode.ToString(),
				currentStatus.ToString(),
				isInProgress ? null : a.Score,
				a.TotalItems,
				a.AnsweredItems,
				isInProgress ? null : a.CorrectItems,
				a.CreatedAt,
				a.ExpiresAt,
				a.CompletedAt,
				a.IsEvaluated);
		}).ToList();

		return Result.Success(new PaginatedList<QuizAttemptSummaryDto>(
			dtoList,
			totalCount,
			request.PageNumber,
			request.PageSize));
	}
}
