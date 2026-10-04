using MediatR;
using Rakushu.Application.Usecases.Learning.MediaAsset;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Video;

namespace Rakushu.Application.Usecases.Learning.MediaAsset.GetMediaAssets;

internal sealed class GetMediaAssetsHandler : IRequestHandler<GetMediaAssetsQuery, Result<IReadOnlyCollection<MediaAssetDto>>>
{
	private readonly IVideoRepository _videoRepository;

	public GetMediaAssetsHandler(IVideoRepository videoRepository)
	{
		_videoRepository = videoRepository;
	}

	public async Task<Result<IReadOnlyCollection<MediaAssetDto>>> Handle(GetMediaAssetsQuery request, CancellationToken cancellationToken)
	{
		var videos = await _videoRepository.GetAllAsync(cancellationToken);
		var assets = videos
			.SelectMany(v => v.MediaAssets)
			.Select(MediaAssetDto.FromEntity)
			.ToArray();

		return Result.Success<IReadOnlyCollection<MediaAssetDto>>(assets);
	}
}