using MediatR;
using Rakushu.Application.Usecases.Material.Transcript;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Video;
using TranscriptEntity = Rakushu.Domain.Entities.Material.Transcript.Transcript;

namespace Rakushu.Application.Usecases.Material.Transcript.CreateTranscript;

internal sealed class CreateTranscriptHandler : IRequestHandler<CreateTranscriptCommand, Result>
{
	private readonly IVideoRepository _videoRepository;
	private readonly IUnitOfWork _unitOfWork;

	public CreateTranscriptHandler(IVideoRepository videoRepository, IUnitOfWork unitOfWork)
	{
		_videoRepository = videoRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(
		CreateTranscriptCommand request,
		CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			// Get video
			var video = await _videoRepository.GetByIdAsync(VideoId.From(request.VideoId), cancellationToken);
			if (video is null)
				return Result.Failure(VideoErrors.NotFound);

			// Check if video already has a transcript
			if (video.Transcript is not null)
				return Result.Failure<TranscriptDto>(TranscriptErrors.AlreadyExists);

			// Create transcript with Official source
			var transcriptResult = TranscriptEntity.Create(
				video.Id,
				request.FullText,
				DateTimeOffset.UtcNow,
				DateTimeOffset.UtcNow);

			if (transcriptResult.IsFailure)
				return Result.Failure(transcriptResult.Error);

			var transcript = transcriptResult.Value;

			// Set storage key for Official source
			transcript.SetStorageKey(request.StorageKey);

			// Assign transcript to video
			video.SetTranscript(transcript);

			return Result.Success();
		}, cancellationToken);
	}
}
