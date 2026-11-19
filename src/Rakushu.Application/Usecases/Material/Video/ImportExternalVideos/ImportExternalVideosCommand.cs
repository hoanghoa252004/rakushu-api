using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.JobLog;
using Rakushu.Application.Abstractions.Infrastructure.VideoImport;
using Rakushu.Domain.Entities.Processing.ProcessingJobLog;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Material.Video.MediaAsset;

namespace Rakushu.Application.Usecases.Material.Video.ImportExternalVideos;

public sealed record ImportExternalVideosCommand(int BatchSize = 1) : IRequest<Result<int>>;

internal sealed class ImportExternalVideosHandler(
	IVideoRepository videoRepository,
	IVideoImportService videoImportService,
	IProcessingJobLogService jobLogService,
	IUnitOfWork unitOfWork)
	: IRequestHandler<ImportExternalVideosCommand, Result<int>>
{
	public async Task<Result<int>> Handle(ImportExternalVideosCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var videos = await videoRepository.GetImportingVideosAsync(request.BatchSize, cancellationToken);

			var imported = 0;
			foreach (var video in videos)
			{
				Guid? jobLogId = null;
				try
				{
					if (string.IsNullOrWhiteSpace(video.SourceUrl))
					{
						video.ChangeStatus(VideoStatus.Rejected);
						continue;
					}

					jobLogId = (await jobLogService.StartAsync(
						ProcessingJobType.YoutubeImport,
						ProcessingJobTargetType.Video,
						video.Id.Value,
						ProcessingJobServices.YtDlp,
						video.SourceUrl,
						cancellationToken)).Id;

					var importResult = await videoImportService.ImportAsync(video.SourceUrl, cancellationToken);

					await jobLogService.CompleteAsync(
						jobLogId.Value,
						JobLogSerializer.Serialize(importResult),
						cancellationToken);

					var mediaAsset = MediaAsset.Create(
						video.Id,
						MediaAssetType.Video,
						importResult.StorageKey,
						importResult.ContentType,
						importResult.FileName,
						importResult.FileSizeBytes);

					video.AddMediaAsset(mediaAsset.Value);

					if (video.ChangeStatus(VideoStatus.Moderating).IsSuccess)
						imported++;
				}
				catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
				{
					if (jobLogId is not null)
						await jobLogService.FailAsync(jobLogId.Value, "Cancelled", CancellationToken.None);
					throw;
				}
				catch (Exception ex)
				{
					if (jobLogId is not null)
						await jobLogService.FailAsync(jobLogId.Value, ex.Message, CancellationToken.None);
					Console.WriteLine($"=== ERROR: {ex.Message}");
					Console.WriteLine($"=== EXCEPTION: {ex}");
					Console.WriteLine($"=== STACK TRACE: {ex.StackTrace}");
				}
			}

			return Result.Success(imported);
		}, cancellationToken);
	}
}
