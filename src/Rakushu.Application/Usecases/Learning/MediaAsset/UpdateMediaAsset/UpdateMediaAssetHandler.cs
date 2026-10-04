using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Video;
using Rakushu.Domain.Entities.Video.MediaAsset;

namespace Rakushu.Application.Usecases.Learning.MediaAsset.UpdateMediaAsset;

internal sealed class UpdateMediaAssetHandler : IRequestHandler<UpdateMediaAssetCommand, Result>
{
	private readonly IVideoRepository _videoRepository;
	private readonly IUnitOfWork _unitOfWork;

	public UpdateMediaAssetHandler(IVideoRepository videoRepository, IUnitOfWork unitOfWork)
	{
		_videoRepository = videoRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(UpdateMediaAssetCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var assetId = MediaAssetId.From(request.MediaAssetId);
			var video = await _videoRepository.GetByMediaAssetIdAsync(assetId, cancellationToken);
			if (video is null)
				return Result.Failure(MediaAssetErrors.NotFound);

			var asset = video.MediaAssets.FirstOrDefault(m => m.Id == assetId);
			if (asset is null)
				return Result.Failure(MediaAssetErrors.NotFound);

			return asset.Update(
				request.assetType,
				request.storageKey,
				request.contentType,
				request.fileName,
				request.fileSizeBytes,
				request.container,
				request.codec,
				request.width,
				request.height,
				request.frameRate);
		}, cancellationToken);
	}
}