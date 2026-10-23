using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Application.Usecases.Material.Video;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Application.Usecases.Material.Video.GetVideoById;

internal sealed class GetVideoByIdHandler : IRequestHandler<GetVideoByIdQuery, Result<VideoDetailDto>>
{
	private readonly IVideoRepository _repository;

	private readonly IStorageService _storageService;

	public GetVideoByIdHandler(IVideoRepository repository, IStorageService storageService)
	{
		_repository = repository;
		_storageService = storageService;
	}

	public async Task<Result<VideoDetailDto>> Handle(GetVideoByIdQuery request, CancellationToken cancellationToken)
	{
		var video = await _repository.GetByIdAsync(VideoId.From(request.VideoId), cancellationToken);

		if (video is null)
			return Result.Failure<VideoDetailDto>(VideoErrors.NotFound);

		var dto = VideoDetailDto.FromEntity(video);

		var mediaAssets = await Task.WhenAll(
			dto.MediaAssets.Select(async mediaAsset =>
			{
				var signedUrl = await _storageService.CreatePresignedReadUrlAsync(
					mediaAsset.StorageKey,
					cancellationToken);

				return mediaAsset with
				{
					StorageKey = signedUrl
				};
			}));

		dto = dto with
		{
			MediaAssets = mediaAssets
		};

		if (dto.Series?.ThumbnailKey != null)
		{
			var thumbnailUrl = await _storageService.CreatePresignedReadUrlAsync(dto.Series.ThumbnailKey, cancellationToken);

			dto = dto with { Series = dto.Series with { ThumbnailKey = thumbnailUrl } };
		}

		return Result.Success(dto);
	}
}