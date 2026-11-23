using MediatR;
using Microsoft.Extensions.Logging;
using Rakushu.Application.Abstractions.Infrastructure.Assessment;
using Rakushu.Application.Abstractions.Infrastructure.JobLog;
using Rakushu.Domain.Entities.Processing.ProcessingJobLog;
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
	IProcessingJobLogService jobLogService,
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

						var accuracy = await RunLoggedAsync(
							ProcessingJobType.PronunciationAssessment,
							detail.Id.Value,
							ProcessingJobServices.AzureSpeech,
							new { detail.ShadowingRoleLine.Text, UserAudioKey = detail.AudioKey },
							() => speechAssessmentService.AssessAccuracyAsync(
								detail.ShadowingRoleLine.Text,
								userUrl,
								cancellationToken),
							r => r.RawResponse,
							cancellationToken);

						decimal? pitchScore = null;
						string? pitchRawResponse = null;

						if (accuracy.Score >= 50m)
						{
							var pitch = await RunLoggedAsync(
								ProcessingJobType.PitchAssessment,
								detail.Id.Value,
								ProcessingJobServices.RakushuEngine,
								new
								{
									detail.ShadowingRoleLine.Text,
									ReferenceAudioKey = detail.ShadowingRoleLine.AudioKey,
									UserAudioKey = detail.AudioKey
								},
								() => speechAssessmentService.AssessPitchSimilarityAsync(
									detail.ShadowingRoleLine.Text,
									referenceUrl,
									userUrl,
									cancellationToken),
								r => r.RawResponse,
								cancellationToken);

							pitchScore = pitch.Score;
							pitchRawResponse = pitch.RawResponse;
						}

						assessment.AddDetail(
							detail.Id,
							accuracy.Score!.Value,
							accuracy.RawResponse,
							pitchScore,
							pitchRawResponse);
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

	private async Task<T> RunLoggedAsync<T>(
		ProcessingJobType jobType,
		Guid attemptDetailId,
		string serviceName,
		object input,
		Func<Task<T>> call,
		Func<T, string?> rawResponse,
		CancellationToken cancellationToken)
	{
		var jobLog = await jobLogService.StartAsync(
			jobType,
			ProcessingJobTargetType.ShadowingAttemptDetail,
			attemptDetailId,
			serviceName,
			JobLogSerializer.Serialize(input),
			cancellationToken);

		T result;
		try
		{
			result = await call();
		}
		catch (Exception ex)
		{
			await jobLogService.FailAsync(jobLog.Id, ex.Message, CancellationToken.None);
			throw;
		}

		await jobLogService.CompleteAsync(jobLog.Id, rawResponse(result), cancellationToken);
		return result;
	}
}
