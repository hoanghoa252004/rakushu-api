using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Video;
using Rakushu.Domain.Entities.Video.MediaAsset;

namespace Rakushu.Application.Usecases.MediaAsset.GetMediaAssetById;

internal sealed class GetMediaAssetByIdHandler : IRequestHandler<GetMediaAssetByIdQuery, Result<MediaAssetDto>>
{
	private readonly IVideoRepository _videoRepository;

	public GetMediaAssetByIdHandler(IVideoRepository videoRepository)
	{
		_videoRepository = videoRepository;
	}

	public async Task<Result<MediaAssetDto>> Handle(GetMediaAssetByIdQuery request, CancellationToken cancellationToken)
	{
		var assetId = MediaAssetId.From(request.MediaAssetId);
		var video = await _videoRepository.GetByMediaAssetIdAsync(assetId, cancellationToken);
		if (video is null)
			return Result.Failure<MediaAssetDto>(MediaAssetErrors.NotFound);

		var asset = video.MediaAssets.FirstOrDefault(m => m.Id == assetId);
		if (asset is null)
			return Result.Failure<MediaAssetDto>(MediaAssetErrors.NotFound);

		return Result.Success(MediaAssetDto.FromEntity(asset));
	}
}