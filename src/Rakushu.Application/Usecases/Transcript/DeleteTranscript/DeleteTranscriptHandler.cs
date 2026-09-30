using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Video;
using Rakushu.Domain.Entities.Video.Transcript;

namespace Rakushu.Application.Usecases.Transcript.DeleteTranscript;

internal sealed class DeleteTranscriptHandler : IRequestHandler<DeleteTranscriptCommand, Result>
{
	private readonly IVideoRepository _videoRepository;
	private readonly IUnitOfWork _unitOfWork;

	public DeleteTranscriptHandler(IVideoRepository videoRepository, IUnitOfWork unitOfWork)
	{
		_videoRepository = videoRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(DeleteTranscriptCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var transcriptId = TranscriptId.From(request.TranscriptId);
			var video = await _videoRepository.GetByTranscriptIdAsync(transcriptId, cancellationToken);
			if (video is null || video.Transcript is null)
				return Result.Failure(TranscriptErrors.NotFound);

			video.RemoveTranscript();
			return Result.Success();
		}, cancellationToken);
	}
}