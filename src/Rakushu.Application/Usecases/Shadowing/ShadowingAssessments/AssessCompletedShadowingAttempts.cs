using MediatR;
using Microsoft.Extensions.Logging;
using Rakushu.Application.Abstractions.Infrastructure.Assessment;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Shadowing.ShadowingAssessment;
using Rakushu.Domain.Entities.Shadowing.ShadowingAttempt;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingAssessments;

// ---- WORKER: AUTOMATED ASSESSMENT ----
public sealed record AssessCompletedShadowingAttemptsCommand(int BatchSize = 5) : IRequest<Result<int>>;

internal sealed class AssessCompletedShadowingAttemptsHandler(
	IShadowingAttemptRepository attemptRepository,
	ISpeechAssessmentService speechAssessmentService,
	IStorageService storageService,
	IUnitOfWork unitOfWork,
	ILogger<AssessCompletedShadowingAttemptsHandler> logger)
	: IRequestHandler<AssessCompletedShadowingAttemptsCommand, Result<int>>
{
	public async Task<Result<int>> Handle(AssessCompletedShadowingAttemptsCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var attempts = await attemptRepository.GetPendingAssessmentAsync(request.BatchSize, cancellationToken);
			var now = DateTimeOffset.UtcNow;
			var count = 0;

			foreach (var attempt in attempts)
			{
				try
				{
					var assessment = ShadowingAssessment.CreateAutomated(attempt.Id, now);

					foreach (var detail in attempt.Details.Where(d => !d.IsSkipped && !string.IsNullOrEmpty(d.AudioKey)))
					{
						var userUrl = await storageService.CreatePresignedReadUrlAsync(detail.AudioKey!, cancellationToken);
						var referenceUrl = await storageService.CreatePresignedReadUrlAsync(detail.ShadowingRoleLine.AudioKey, cancellationToken);

						var accuracy = await speechAssessmentService.AssessAccuracyAsync(
							detail.ShadowingRoleLine.Text, userUrl, cancellationToken);
						var pitch = await speechAssessmentService.AssessPitchSimilarityAsync(
							referenceUrl, userUrl, cancellationToken);

						assessment.AddDetail(detail.Id, accuracy.Score, accuracy.RawResponse, pitch.Score, pitch.RawResponse);
					}

					assessment.CalculateOverallScore();
					attemptRepository.AddAssessment(assessment);
					attempt.MarkAssessed();
					count++;
				}
				catch (Exception ex) when (ex is not OperationCanceledException)
				{
					logger.LogError(ex, "Failed to assess shadowing attempt {AttemptId}.", attempt.Id.Value);
				}
			}

			return Result.Success(count);
		}, cancellationToken);
	}
}
