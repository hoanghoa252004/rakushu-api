using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Application.Usecases.Material.Transcript.MarkTranscriptReviewed;

internal sealed class MarkTranscriptReviewedHandler : IRequestHandler<MarkTranscriptReviewedCommand, Result>
{
	private readonly IVideoRepository _videoRepository;
	private readonly IUnitOfWork _unitOfWork;

	public MarkTranscriptReviewedHandler(IVideoRepository videoRepository, IUnitOfWork unitOfWork)
	{
		_videoRepository = videoRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(
		MarkTranscriptReviewedCommand request,
		CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var video = await _videoRepository.GetByIdAsync(VideoId.From(request.VideoId), cancellationToken);
			if (video is null)
				return Result.Failure(VideoErrors.NotFound);

			if (video.Transcript is null)
				return Result.Failure(TranscriptErrors.NotFound);

			video.Transcript.MarkAsReviewed();

			return Result.Success();
		}, cancellationToken);
	}
}
