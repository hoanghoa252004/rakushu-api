using MediatR;
using Microsoft.Extensions.Logging;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Srs.QuizAttempt;

namespace Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.EvaluatePendingQuizAttempts;

internal sealed class EvaluatePendingQuizAttemptsHandler(
	IQuizAttemptRepository attemptRepository,
	IQuizAttemptFeedbackService feedbackService,
	ILogger<EvaluatePendingQuizAttemptsHandler> logger) : IRequestHandler<EvaluatePendingQuizAttemptsCommand, Result<int>>
{
	public async Task<Result<int>> Handle(
		EvaluatePendingQuizAttemptsCommand request,
		CancellationToken cancellationToken)
	{
		var pendingIds = await attemptRepository.GetPendingEvaluationAttemptIdsAsync(request.BatchSize, cancellationToken);
		if (pendingIds.Count == 0)
			return Result.Success(0);

		var evaluatedCount = 0;
		foreach (var attemptId in pendingIds)
		{
			if (cancellationToken.IsCancellationRequested)
				break;

			try
			{
				await feedbackService.GenerateFeedbackAsync(attemptId, cancellationToken);
				evaluatedCount++;
			}
			catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
			{
				break;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Failed to generate adaptive feedback for quiz attempt {AttemptId}", attemptId.Value);
			}
		}

		return Result.Success(evaluatedCount);
	}
}
