using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Video;
using Rakushu.Domain.Entities.Video.MediaAsset;

namespace Rakushu.Application.Usecases.MediaAsset.DeleteMediaAsset;

internal sealed class DeleteMediaAssetHandler : IRequestHandler<DeleteMediaAssetCommand, Result>
{
	private readonly IVideoRepository _videoRepository;
	private readonly IUnitOfWork _unitOfWork;

	public DeleteMediaAssetHandler(IVideoRepository videoRepository, IUnitOfWork unitOfWork)
	{
		_videoRepository = videoRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(DeleteMediaAssetCommand request, CancellationToken cancellationToken)
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

			video.RemoveMediaAsset(asset);
			return Result.Success();
		}, cancellationToken);
	}
}