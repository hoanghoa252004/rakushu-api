using MediatR;
using Rakushu.Application.Usecases.Learning.Transcript;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Video;
using Rakushu.Domain.Entities.Video.Transcript;

namespace Rakushu.Application.Usecases.Learning.Transcript.GetTranscriptById;

internal sealed class GetTranscriptByIdHandler : IRequestHandler<GetTranscriptByIdQuery, Result<TranscriptDto>>
{
	private readonly IVideoRepository _videoRepository;

	public GetTranscriptByIdHandler(IVideoRepository videoRepository)
	{
		_videoRepository = videoRepository;
	}

	public async Task<Result<TranscriptDto>> Handle(GetTranscriptByIdQuery request, CancellationToken cancellationToken)
	{
		var transcriptId = TranscriptId.From(request.TranscriptId);
		var video = await _videoRepository.GetByTranscriptIdAsync(transcriptId, cancellationToken);
		if (video is null || video.Transcript is null)
			return Result.Failure<TranscriptDto>(TranscriptErrors.NotFound);

		return Result.Success(TranscriptDto.FromEntity(video.Transcript));
	}
}