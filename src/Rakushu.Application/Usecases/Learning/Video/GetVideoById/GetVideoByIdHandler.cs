using MediatR;
using Rakushu.Application.Usecases.Learning.Video;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Video;

namespace Rakushu.Application.Usecases.Learning.Video.GetVideoById;

internal sealed class GetVideoByIdHandler : IRequestHandler<GetVideoByIdQuery, Result<VideoDto>>
{
	private readonly IVideoRepository _repository;

	public GetVideoByIdHandler(IVideoRepository repository)
	{
		_repository = repository;
	}

	public async Task<Result<VideoDto>> Handle(GetVideoByIdQuery request, CancellationToken cancellationToken)
	{
		var video = await _repository.GetByIdAsync(VideoId.From(request.VideoId), cancellationToken);
		if (video is null)
			return Result.Failure<VideoDto>(VideoErrors.NotFound);

		return Result.Success(VideoDto.FromEntity(video));
	}
}