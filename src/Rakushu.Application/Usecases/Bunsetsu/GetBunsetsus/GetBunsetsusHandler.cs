using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.LearningUnit.Bunsetsu;
using Rakushu.Domain.Entities.Video;

namespace Rakushu.Application.Usecases.Bunsetsu.GetBunsetsus;

internal sealed class GetBunsetsusHandler : IRequestHandler<GetBunsetsusQuery, Result<IReadOnlyCollection<BunsetsuDto>>>
{
	private readonly IVideoRepository _videoRepository;

	public GetBunsetsusHandler(IVideoRepository videoRepository)
	{
		_videoRepository = videoRepository;
	}

	public async Task<Result<IReadOnlyCollection<BunsetsuDto>>> Handle(GetBunsetsusQuery request, CancellationToken cancellationToken)
	{
		var videos = await _videoRepository.GetAllAsync(cancellationToken);
		var bunsetsus = videos
			.Where(v => v.Transcript != null)
			.SelectMany(v => v.Transcript!.TranscriptSegments)
			.SelectMany(s => s.Bunsetsu)
			.Select(BunsetsuDto.FromEntity)
			.ToArray();

		return Result.Success<IReadOnlyCollection<BunsetsuDto>>(bunsetsus);
	}
}