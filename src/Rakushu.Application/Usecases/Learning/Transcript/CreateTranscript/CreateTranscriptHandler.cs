using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Video;
using Rakushu.Domain.Entities.Video.Transcript;

namespace Rakushu.Application.Usecases.Learning.Transcript.CreateTranscript;

internal sealed class CreateTranscriptHandler : IRequestHandler<CreateTranscriptCommand, Result<Guid>>
{
	private readonly IVideoRepository _videoRepository;
	private readonly IUnitOfWork _unitOfWork;

	public CreateTranscriptHandler(IVideoRepository videoRepository, IUnitOfWork unitOfWork)
	{
		_videoRepository = videoRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<Guid>> Handle(CreateTranscriptCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var video = await _videoRepository.GetByIdAsync(VideoId.From(request.videoId), cancellationToken);
			if (video is null)
				return Result.Failure<Guid>(VideoErrors.NotFound);

			var now = DateTimeOffset.UtcNow;
			var transcriptResult = Domain.Entities.Video.Transcript.Transcript.Create(
				video.Id,
				request.fullText,
				now,
				now);

			if (transcriptResult.IsFailure)
				return Result.Failure<Guid>(transcriptResult.Error);

			video.SetTranscript(transcriptResult.Value);
			return Result.Success(transcriptResult.Value.Id.Value);
		}, cancellationToken);
	}
}