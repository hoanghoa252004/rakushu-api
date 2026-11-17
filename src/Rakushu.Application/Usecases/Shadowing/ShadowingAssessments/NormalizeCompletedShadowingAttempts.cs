using MediatR;
using Microsoft.Extensions.Logging;
using Rakushu.Application.Abstractions.Infrastructure.AudioClips;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Shadowing.ShadowingAttempt;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingAssessments;

// ---- WORKER: AUDIO NORMALIZATION ----
public sealed record NormalizeCompletedShadowingAttemptsCommand(int BatchSize = 5) : IRequest<Result<int>>;

internal sealed class NormalizeCompletedShadowingAttemptsHandler(
	IShadowingAttemptRepository attemptRepository,
	IMediaConvertService mediaConvertService,
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
						var jobs = await mediaConvertService.ListNormalizeJobsAsync(attempt.Id.Value, cancellationToken);

						foreach (var detail in pending)
						{
							var lineId = detail.ShadowingRoleLineId.Value;
							var lineJobs = jobs.Where(j => j.RoleLineId == lineId).ToList();
							var outputKey = $"shadowing/normalized/{attempt.Id.Value}/{lineId}";

							if (lineJobs.Any(j => j.Status == NormalizeJobStatus.Complete))
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

							await mediaConvertService.CreateNormalizeJobAsync(
								attempt.Id.Value, lineId, detail.AudioKey!, outputKey, cancellationToken);
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
}
