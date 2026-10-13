using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Application.Usecases.Learning.Transcript.DeleteTranscript;

/// <summary>
/// Handles deletion of a transcript from a video.
/// Only allows deletion if the video is NOT in Ready status.
/// If video is Ready, only UPDATE is allowed.
/// </summary>
internal sealed class DeleteTranscriptHandler : IRequestHandler<DeleteTranscriptCommand, Result>
{
	private readonly IVideoRepository _videoRepository;
	private readonly IUnitOfWork _unitOfWork;

	public DeleteTranscriptHandler(IVideoRepository videoRepository, IUnitOfWork unitOfWork)
	{
		_videoRepository = videoRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(
		DeleteTranscriptCommand request,
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

			// Check if video is in Ready status - cannot delete transcript if video is Ready
			if (video.Status == VideoStatus.Ready)
				return Result.Failure(TranscriptErrors.CannotDeleteReadyVideo);

			// Remove transcript from video
			video.RemoveTranscript();

			return Result.Success();
		}, cancellationToken);
	}
}
