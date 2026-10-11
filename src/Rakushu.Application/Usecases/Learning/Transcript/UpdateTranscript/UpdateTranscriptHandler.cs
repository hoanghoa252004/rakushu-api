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

	public async Task<Result> Handle(
		UpdateTranscriptCommand request,
		CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			// Get video
			var video = await _videoRepository.GetByIdAsync(VideoId.From(request.VideoId), cancellationToken);
			if (video is null)
				return Result.Failure(VideoErrors.NotFound);

			// Check if transcript exists
			if (video.Transcript is null)
				return Result.Failure(TranscriptErrors.NotFound);

			// Update transcript (FullText and StorageKey only)
			var updateResult = video.Transcript.UpdateTranscript(
				request.FullText,
				request.StorageKey,
				DateTimeOffset.UtcNow);

			if (updateResult.IsFailure)
				return Result.Failure(updateResult.Error);

			return Result.Success();
		}, cancellationToken);
	}
}
