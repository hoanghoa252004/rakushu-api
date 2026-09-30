using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Video;

namespace Rakushu.Application.Usecases.Transcript.GetTranscripts;

internal sealed class GetTranscriptsHandler : IRequestHandler<GetTranscriptsQuery, Result<IReadOnlyCollection<TranscriptDto>>>
{
	private readonly IVideoRepository _videoRepository;

	public GetTranscriptsHandler(IVideoRepository videoRepository)
	{
		_videoRepository = videoRepository;
	}

	public async Task<Result<IReadOnlyCollection<TranscriptDto>>> Handle(GetTranscriptsQuery request, CancellationToken cancellationToken)
	{
		var videos = await _videoRepository.GetAllAsync(cancellationToken);
		var transcripts = videos
			.Where(v => v.Transcript != null)
			.Select(v => TranscriptDto.FromEntity(v.Transcript!))
			.ToArray();

		return Result.Success<IReadOnlyCollection<TranscriptDto>>(transcripts);
	}
}