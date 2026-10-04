using MediatR;
using Rakushu.Application.Usecases.Learning.Video;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Video;

namespace Rakushu.Application.Usecases.Learning.Video.GetVideos;

internal sealed class GetVideosHandler : IRequestHandler<GetVideosQuery, Result<IReadOnlyCollection<VideoDto>>>
{
	private readonly IVideoRepository _repository;

	public GetVideosHandler(IVideoRepository repository)
	{
		_repository = repository;
	}

	public async Task<Result<IReadOnlyCollection<VideoDto>>> Handle(GetVideosQuery request, CancellationToken cancellationToken)
	{
		var list = await _repository.GetAllAsync(cancellationToken);
		return Result.Success<IReadOnlyCollection<VideoDto>>(list.Select(VideoDto.FromEntity).ToArray());
	}
}