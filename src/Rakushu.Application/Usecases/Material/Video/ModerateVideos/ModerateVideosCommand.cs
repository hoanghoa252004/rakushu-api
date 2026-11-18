using MediatR;
using Microsoft.Extensions.Logging;
using Rakushu.Application.Abstractions.Infrastructure.Moderation;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Material.Video.MediaAsset;

namespace Rakushu.Application.Usecases.Material.Video.ModerateVideos;

public sealed record ModerateVideosCommand(int BatchSize = 10) : IRequest<Result<int>>;

internal sealed class ModerateVideosHandler(
	IVideoRepository videoRepository,
	IVideoModerationService moderationService,
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

					var status = await moderationService.CheckAsync(video.Id.Value, asset.StorageKey, cancellationToken);

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
}
