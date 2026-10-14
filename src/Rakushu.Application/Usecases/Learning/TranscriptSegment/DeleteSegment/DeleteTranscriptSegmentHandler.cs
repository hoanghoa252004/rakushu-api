using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Application.Usecases.Learning.TranscriptSegment.DeleteSegment;

/// <summary>
/// Handles deletion of a transcript segment with automatic sequence reordering.
/// When a segment is deleted, all subsequent segments automatically have their
/// sequence numbers decremented to maintain consistency.
/// </summary>
internal sealed class DeleteTranscriptSegmentHandler : IRequestHandler<DeleteTranscriptSegmentCommand, Result>
{
	private readonly IVideoRepository _videoRepository;

	public DeleteTranscriptSegmentHandler(IVideoRepository videoRepository)
	{
		_videoRepository = videoRepository;
	}

	public async Task<Result> Handle(
		DeleteTranscriptSegmentCommand request,
		CancellationToken cancellationToken)
	{
		// Get the video
		var video = await _videoRepository.GetByIdAsync(VideoId.From(request.VideoId), cancellationToken);
		if (video is null)
			return Result.Failure(VideoErrors.NotFound);

		// Get the transcript
		if (video.Transcript is null)
			return Result.Failure(TranscriptErrors.NotFound);

		// Get the segment to delete
		var segmentId = TranscriptSegmentId.From(request.SegmentId);
		var segment = video.Transcript.TranscriptSegments.FirstOrDefault(s => s.Id == segmentId);
		if (segment is null)
			return Result.Failure(TranscriptSegmentErrors.NotFound);

		// Delete the segment - RemoveSegment() handles automatic sequence reordering in the domain
		// All segments with sequence > deletedSegment.Sequence will be decremented by 1
		var removeResult = video.Transcript.RemoveSegment(segmentId);
		if (removeResult.IsFailure)
			return removeResult;

		return Result.Success();
	}
}


