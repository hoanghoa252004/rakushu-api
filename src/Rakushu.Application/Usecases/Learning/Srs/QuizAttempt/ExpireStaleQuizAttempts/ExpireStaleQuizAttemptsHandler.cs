using MediatR;
using Microsoft.Extensions.Logging;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.SubmitQuizAttempt;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Srs.QuizAttempt;

namespace Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.ExpireStaleQuizAttempts;

internal sealed class ExpireStaleQuizAttemptsHandler : IRequestHandler<ExpireStaleQuizAttemptsCommand, Result<int>>
{
	private readonly IQuizAttemptRepository _quizAttemptRepository;
	private readonly IQuizAttemptSrsService _quizAttemptSrsService;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ILogger<ExpireStaleQuizAttemptsHandler> _logger;

	public ExpireStaleQuizAttemptsHandler(
		IQuizAttemptRepository quizAttemptRepository,
		IQuizAttemptSrsService quizAttemptSrsService,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork,
		ILogger<ExpireStaleQuizAttemptsHandler> logger)
	{
		_quizAttemptRepository = quizAttemptRepository;
		_quizAttemptSrsService = quizAttemptSrsService;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
		_logger = logger;
	}

	public async Task<Result<int>> Handle(
		ExpireStaleQuizAttemptsCommand request,
		CancellationToken cancellationToken)
	{
		var now = _systemClock.UtcNow;
		var staleAttempts = await _quizAttemptRepository.GetStaleAttemptsAsync(
			now,
			request.BatchSize,
			cancellationToken);

		if (staleAttempts.Count == 0)
			return Result.Success(0);

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			int expiredCount = 0;
			foreach (var attempt in staleAttempts)
			{
				if (cancellationToken.IsCancellationRequested)
					break;

				try
				{
					attempt.Expire(now);
					if (attempt.HasAnyAnsweredItems)
					{
						await _quizAttemptSrsService.ApplySrsUpdatesAsync(attempt, now, cancellationToken);
					}
					_quizAttemptRepository.Update(attempt);
					expiredCount++;
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Failed to expire stale quiz attempt {AttemptId}", attempt.Id.Value);
				}
			}

			return Result.Success(expiredCount);
		}, cancellationToken);
	}
}
