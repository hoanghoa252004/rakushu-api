using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Application.Usecases.Material.TranscriptSegment.DeleteSegment;

/// <summary>
/// Handles deletion of a transcript segment with automatic sequence reordering.
/// When a segment is deleted, all subsequent segments automatically have their
/// sequence numbers decremented to maintain consistency.
/// </summary>
internal sealed class DeleteTranscriptSegmentHandler : IRequestHandler<DeleteTranscriptSegmentCommand, Result>
{
	private readonly IVideoRepository _videoRepository;
	private readonly IUnitOfWork _unitOfWork;

	public DeleteTranscriptSegmentHandler(IVideoRepository videoRepository, IUnitOfWork unitOfWork)
	{
		_videoRepository = videoRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(
		DeleteTranscriptSegmentCommand request,
		CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var video = await _videoRepository.GetByIdAsync(VideoId.From(request.VideoId), cancellationToken);
			if (video is null)
				return Result.Failure(VideoErrors.NotFound);

			if (video.Transcript is null)
				return Result.Failure(TranscriptErrors.NotFound);

			var segmentId = TranscriptSegmentId.From(request.SegmentId);
			var segment = video.Transcript.TranscriptSegments.FirstOrDefault(s => s.Id == segmentId);
			if (segment is null)
				return Result.Failure(TranscriptSegmentErrors.NotFound);

			// RemoveSegment() decrements the sequence of all following segments in the domain
			return video.Transcript.RemoveSegment(segmentId);
		}, cancellationToken);
	}
}


