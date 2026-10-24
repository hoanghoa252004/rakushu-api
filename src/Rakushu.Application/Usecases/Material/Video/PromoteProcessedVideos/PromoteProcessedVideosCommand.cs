using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Application.Usecases.Material.Video.PromoteProcessedVideos;

public sealed record PromoteProcessedVideosCommand(int BatchSize = 10) : IRequest<Result<int>>;

internal sealed class PromoteProcessedVideosHandler(IVideoRepository videoRepository, IUnitOfWork unitOfWork)
	: IRequestHandler<PromoteProcessedVideosCommand, Result<int>>
{
	public async Task<Result<int>> Handle(PromoteProcessedVideosCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var videos = await videoRepository.GetProcessingVideosWithProcessedResultAsync(
				request.BatchSize, cancellationToken);

			var updated = 0;
			foreach (var video in videos)
			{
				var result = video.ChangeStatus(VideoStatus.Reviewing);
				if (result.IsSuccess)
					updated++;
			}

			return Result.Success(updated);
		}, cancellationToken);
	}
}
