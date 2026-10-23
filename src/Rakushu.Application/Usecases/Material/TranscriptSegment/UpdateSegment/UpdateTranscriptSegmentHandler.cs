using MediatR;
using Rakushu.Application.Usecases.Material.Transcript;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Application.Usecases.Material.TranscriptSegment.UpdateSegment;

internal sealed class UpdateTranscriptSegmentHandler : IRequestHandler<UpdateTranscriptSegmentCommand, Result>
	{
		private readonly IVideoRepository _videoRepository;

		public UpdateTranscriptSegmentHandler(IVideoRepository videoRepository)
		{
			_videoRepository = videoRepository;
		}

	public async Task<Result> Handle(
		UpdateTranscriptSegmentCommand request,
		CancellationToken cancellationToken)
	{
		var video = await _videoRepository.GetByIdAsync(VideoId.From(request.VideoId), cancellationToken);
		if (video is null)
			return Result.Failure(VideoErrors.NotFound);

		if (video.Transcript is null)
			return Result.Failure(
				TranscriptErrors.NotFound);

		var segmentId = TranscriptSegmentId.From(request.SegmentId);
		var segment = video.Transcript.TranscriptSegments.FirstOrDefault(s => s.Id == segmentId);
		if (segment is null)
			return Result.Failure(TranscriptSegmentErrors.NotFound);

		// Handle sequence change if provided
		if (request.Sequence.HasValue && request.Sequence.Value != segment.Sequence)
		{
			var updateSequenceResult = video.Transcript.UpdateSegmentSequence(segmentId, request.Sequence.Value);
			if (updateSequenceResult.IsFailure)
				return Result.Failure(updateSequenceResult.Error);
		}

		// Update segment content
		var updateResult = segment.Update(
			request.Text,
			request.StartTime,
			request.EndTime,
			request.Sequence ?? segment.Sequence,
			DateTimeOffset.UtcNow);

		if (updateResult.IsFailure)
			return Result.Failure(updateResult.Error);

		return Result.Success();
	}
}
