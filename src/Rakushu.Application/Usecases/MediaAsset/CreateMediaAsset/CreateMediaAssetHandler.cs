using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Video;
using Rakushu.Domain.Entities.Video.MediaAsset;

namespace Rakushu.Application.Usecases.MediaAsset.CreateMediaAsset;

internal sealed class CreateMediaAssetHandler : IRequestHandler<CreateMediaAssetCommand, Result<Guid>>
{
	private readonly IVideoRepository _videoRepository;
	private readonly IUnitOfWork _unitOfWork;

	public CreateMediaAssetHandler(IVideoRepository videoRepository, IUnitOfWork unitOfWork)
	{
		_videoRepository = videoRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<Guid>> Handle(CreateMediaAssetCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var video = await _videoRepository.GetByIdAsync(VideoId.From(request.videoId), cancellationToken);
			if (video is null)
				return Result.Failure<Guid>(VideoErrors.NotFound);

			var assetResult = Domain.Entities.Video.MediaAsset.MediaAsset.Create(
				video.Id,
				request.assetType,
				request.storageKey,
				request.contentType,
				request.fileName,
				request.fileSizeBytes,
				DateTime.UtcNow,
				request.container,
				request.codec,
				request.width,
				request.height,
				request.frameRate);

			if (assetResult.IsFailure)
				return Result.Failure<Guid>(assetResult.Error);

			video.AddMediaAsset(assetResult.Value);
			return Result.Success(assetResult.Value.Id.Value);
		}, cancellationToken);
	}
}