using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Video;
using Rakushu.Domain.Entities.Video.Transcript;

namespace Rakushu.Application.Usecases.Learning.Transcript.UpdateTranscript;

internal sealed class UpdateTranscriptHandler : IRequestHandler<UpdateTranscriptCommand, Result>
{
	private readonly IVideoRepository _videoRepository;
	private readonly IUnitOfWork _unitOfWork;

	public UpdateTranscriptHandler(IVideoRepository videoRepository, IUnitOfWork unitOfWork)
	{
		_videoRepository = videoRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(UpdateTranscriptCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var transcriptId = TranscriptId.From(request.TranscriptId);
			var video = await _videoRepository.GetByTranscriptIdAsync(transcriptId, cancellationToken);
			if (video is null || video.Transcript is null)
				return Result.Failure(TranscriptErrors.NotFound);

			var now = DateTimeOffset.UtcNow;
			return video.Transcript.Update(request.fullText, now);
		}, cancellationToken);
	}
}