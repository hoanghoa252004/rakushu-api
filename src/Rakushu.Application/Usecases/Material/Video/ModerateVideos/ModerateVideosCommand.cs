using MediatR;
using Microsoft.Extensions.Logging;
using Rakushu.Application.Abstractions.Infrastructure.JobLog;
using Rakushu.Application.Abstractions.Infrastructure.Moderation;
using Rakushu.Domain.Entities.Processing.ProcessingJobLog;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Material.Video.MediaAsset;

namespace Rakushu.Application.Usecases.Material.Video.ModerateVideos;

public sealed record ModerateVideosCommand(int BatchSize = 10) : IRequest<Result<int>>;

internal sealed class ModerateVideosHandler(
	IVideoRepository videoRepository,
	IVideoModerationService moderationService,
	IProcessingJobLogService jobLogService,
	IUnitOfWork unitOfWork,
	ILogger<ModerateVideosHandler> logger)
	: IRequestHandler<ModerateVideosCommand, Result<int>>
{
	private const string JapaneseLanguageCode = "ja-JP";

	public async Task<Result<int>> Handle(ModerateVideosCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var videos = await videoRepository.GetModeratingVideosAsync(request.BatchSize, cancellationToken);
			var decided = 0;

			foreach (var video in videos)
			{
				cancellationToken.ThrowIfCancellationRequested();

				try
				{
					var asset = video.MediaAssets.FirstOrDefault(a => a.AssetType == MediaAssetType.Video);
					if (asset is null)
					{
						logger.LogWarning("Video {VideoId} has no video asset; rejecting.", video.Id.Value);
						video.ChangeStatus(VideoStatus.Rejected);
						decided++;
						continue;
					}

					var languageLog = await jobLogService.GetOrStartAsync(
						ProcessingJobType.LanguageModeration,
						ProcessingJobTargetType.Video,
						video.Id.Value,
						ProcessingJobServices.AwsTranscribe,
						asset.StorageKey,
						cancellationToken);

					var contentLog = await jobLogService.GetOrStartAsync(
						ProcessingJobType.ContentModeration,
						ProcessingJobTargetType.Video,
						video.Id.Value,
						ProcessingJobServices.AwsRekognition,
						asset.StorageKey,
						cancellationToken);

					var language = await moderationService.CheckLanguageAsync(languageLog.Id, asset.StorageKey, cancellationToken);
					await TrackAsync(languageLog, language.State, language.JobLabel, language.RawResponse, cancellationToken);

					var content = await moderationService.CheckContentAsync(contentLog.Id, asset.StorageKey, cancellationToken);
					await TrackAsync(contentLog, content.State, content.JobLabel, content.RawResponse, cancellationToken);

					var status = new VideoModerationStatus(language, content);

					// Wait until both jobs have a result.
					if (!status.IsCompleted)
						continue;

					var isJapanese = status.Language.State == ModerationJobState.Completed &&
						status.Language.LanguageCode == JapaneseLanguageCode;
					var isSafe = status.Content.State == ModerationJobState.Completed &&
						status.Content.FlaggedLabels.Count == 0;

					if (isJapanese && isSafe)
					{
						video.ChangeStatus(VideoStatus.Pending);
					}
					else
					{
						logger.LogInformation(
							"Video {VideoId} rejected. Language: {State}/{Language}/{LangReason}; Content: {ContentState}/[{Labels}]/{ContentReason}",
							video.Id.Value,
							status.Language.State, status.Language.LanguageCode, status.Language.FailureReason,
							status.Content.State, string.Join(", ", status.Content.FlaggedLabels), status.Content.FailureReason);
						video.ChangeStatus(VideoStatus.Rejected);
					}

					decided++;
				}
				catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
				{
					throw;
				}
				catch (Exception ex)
				{
					logger.LogError(ex, "Failed to moderate video {VideoId}.", video.Id.Value);
				}
			}

			return Result.Success(decided);
		}, cancellationToken);
	}

	private async Task TrackAsync(
		ProcessingJobLogInfo log,
		ModerationJobState state,
		string? jobLabel,
		string? rawResponse,
		CancellationToken cancellationToken)
	{
		if (log.IsFinished)
			return;

		if (!string.IsNullOrWhiteSpace(jobLabel) && log.JobLabel != jobLabel)
			await jobLogService.SetJobLabelAsync(log.Id, jobLabel, cancellationToken);

		if (state == ModerationJobState.Completed)
			await jobLogService.CompleteAsync(log.Id, rawResponse, cancellationToken);
		else if (state == ModerationJobState.Failed)
			await jobLogService.FailAsync(log.Id, rawResponse, cancellationToken);
	}
}
