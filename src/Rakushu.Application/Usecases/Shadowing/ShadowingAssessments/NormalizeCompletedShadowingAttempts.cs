using MediatR;
using Microsoft.Extensions.Logging;
using Rakushu.Application.Abstractions.Infrastructure.AudioClips;
using Rakushu.Application.Abstractions.Infrastructure.JobLog;
using Rakushu.Domain.Entities.Processing.ProcessingJobLog;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Shadowing.ShadowingAttempt;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingAssessments;

// ---- WORKER: AUDIO NORMALIZATION ----
public sealed record NormalizeCompletedShadowingAttemptsCommand(int BatchSize = 5) : IRequest<Result<int>>;

internal sealed class NormalizeCompletedShadowingAttemptsHandler(
	IShadowingAttemptRepository attemptRepository,
	IMediaConvertService mediaConvertService,
	IProcessingJobLogService jobLogService,
	IUnitOfWork unitOfWork,
	ILogger<NormalizeCompletedShadowingAttemptsHandler> logger)
	: IRequestHandler<NormalizeCompletedShadowingAttemptsCommand, Result<int>>
{
	public async Task<Result<int>> Handle(NormalizeCompletedShadowingAttemptsCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var attempts = await attemptRepository.GetPendingNormalizationAsync(request.BatchSize, cancellationToken);
			var count = 0;

			foreach (var attempt in attempts)
			{
				try
				{
					var pending = attempt.Details
						.Where(d => !d.IsSkipped
							&& !string.IsNullOrWhiteSpace(d.AudioKey)
							&& !d.AudioKey!.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
						.ToList();

					if (pending.Count > 0)
					{
						// Each normalize job has its own job log targeting the attempt detail.
						var logs = await jobLogService.GetByTargetsAsync(
							ProcessingJobType.ShadowingAudioNormalization,
							pending.Select(d => d.Id.Value).ToList(),
							cancellationToken);
						var jobs = await mediaConvertService.ListNormalizeJobsAsync(
							logs.Select(l => l.Id).ToList(), cancellationToken);

						foreach (var detail in pending)
						{
							var lineId = detail.ShadowingRoleLineId.Value;
							var detailLogs = logs.Where(l => l.TargetId == detail.Id.Value).ToList();
							var detailLogIds = detailLogs.Select(l => l.Id).ToHashSet();
							var lineJobs = jobs.Where(j => detailLogIds.Contains(j.JobLogId)).ToList();
							var outputKey = $"shadowing/normalized/{attempt.Id.Value}/{lineId}";

							await SyncLogsAsync(detailLogs, lineJobs, cancellationToken);

							if (detailLogs.Any(l => l.IsSuccess && l.EndAt is not null)
								|| lineJobs.Any(j => j.Status == NormalizeJobStatus.Complete))
							{
								detail.ReplaceAudioKey(outputKey + ".wav");
								continue;
							}

							if (lineJobs.Any(j => j.Status == NormalizeJobStatus.InProgress))
								continue;

							if (lineJobs.Count > 0)
								logger.LogWarning(
									"Normalize job failed for attempt {AttemptId}, role line {RoleLineId}: {Error}. Retrying.",
									attempt.Id.Value, lineId, lineJobs[0].Error);

							var jobLog = await jobLogService.StartAsync(
								ProcessingJobType.ShadowingAudioNormalization,
								ProcessingJobTargetType.ShadowingAttemptDetail,
								detail.Id.Value,
								ProcessingJobServices.AwsMediaConvert,
								detail.AudioKey,
								cancellationToken);

							try
							{
								var externalJobId = await mediaConvertService.CreateNormalizeJobAsync(
									jobLog.Id, detail.AudioKey!, outputKey, cancellationToken);
								await jobLogService.SetJobLabelAsync(jobLog.Id, externalJobId, cancellationToken);
							}
							catch (Exception ex) when (ex is not OperationCanceledException)
							{
								await jobLogService.FailAsync(jobLog.Id, ex.Message, CancellationToken.None);
								throw;
							}
						}
					}

					var allDone = attempt.Details.All(d => d.IsSkipped
						|| string.IsNullOrWhiteSpace(d.AudioKey)
						|| d.AudioKey!.EndsWith(".wav", StringComparison.OrdinalIgnoreCase));

					if (allDone && attempt.MarkNormalized().IsSuccess)
						count++;
				}
				catch (Exception ex) when (ex is not OperationCanceledException)
				{
					logger.LogError(ex, "Failed to normalize shadowing attempt {AttemptId}.", attempt.Id.Value);
				}
			}

			return Result.Success(count);
		}, cancellationToken);
	}

	private async Task SyncLogsAsync(
		IReadOnlyList<ProcessingJobLogInfo> logs,
		IReadOnlyList<NormalizeJobInfo> jobs,
		CancellationToken ct)
	{
		foreach (var log in logs.Where(l => !l.IsFinished))
		{
			var job = jobs.FirstOrDefault(j => j.JobLogId == log.Id);
			if (job is null)
				continue;

			if (job.Status == NormalizeJobStatus.Complete)
				await jobLogService.CompleteAsync(log.Id, job.RawResponse, ct);
			else if (job.Status == NormalizeJobStatus.Failed)
				await jobLogService.FailAsync(log.Id, job.RawResponse ?? job.Error, ct);
		}
	}
}
