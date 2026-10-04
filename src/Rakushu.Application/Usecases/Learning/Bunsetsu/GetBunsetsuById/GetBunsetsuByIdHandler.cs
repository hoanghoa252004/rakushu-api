using MediatR;
using Rakushu.Application.Usecases.Learning.Bunsetsu;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.LearningUnit.Bunsetsu;
using Rakushu.Domain.Entities.Video;

namespace Rakushu.Application.Usecases.Learning.Bunsetsu.GetBunsetsuById;

internal sealed class GetBunsetsuByIdHandler : IRequestHandler<GetBunsetsuByIdQuery, Result<BunsetsuDto>>
{
	private readonly IVideoRepository _videoRepository;

	public GetBunsetsuByIdHandler(IVideoRepository videoRepository)
	{
		_videoRepository = videoRepository;
	}

	public async Task<Result<BunsetsuDto>> Handle(GetBunsetsuByIdQuery request, CancellationToken cancellationToken)
	{
		var bunsetsuId = BunsetsuId.From(request.BunsetsuId);
		var video = await _videoRepository.GetByBunsetsuIdAsync(bunsetsuId, cancellationToken);
		if (video is null || video.Transcript is null)
			return Result.Failure<BunsetsuDto>(BunsetsuErrors.NotFound);

		var bunsetsu = video.Transcript.TranscriptSegments
			.SelectMany(s => s.Bunsetsu)
			.FirstOrDefault(b => b.Id == bunsetsuId);

		if (bunsetsu is null)
			return Result.Failure<BunsetsuDto>(BunsetsuErrors.NotFound);

		return Result.Success(BunsetsuDto.FromEntity(bunsetsu));
	}
}