using MediatR;
using Rakushu.Application.Usecases.Learning.Transcript;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Application.Usecases.Learning.TranscriptSegment.UpdateSegment;

internal sealed class UpdateTranscriptSegmentHandler : IRequestHandler<UpdateTranscriptSegmentCommand, Result<TranscriptSegmentDetailDto>>
	{
		private readonly IVideoRepository _videoRepository;

		public UpdateTranscriptSegmentHandler(IVideoRepository videoRepository)
		{
			_videoRepository = videoRepository;
		}

	public async Task<Result<TranscriptSegmentDetailDto>> Handle(
		UpdateTranscriptSegmentCommand request,
		CancellationToken cancellationToken)
	{
		var video = await _videoRepository.GetByIdAsync(VideoId.From(request.VideoId), cancellationToken);
		if (video is null)
			return Result.Failure<TranscriptSegmentDetailDto>(VideoErrors.NotFound);

		if (video.Transcript is null)
			return Result.Failure<TranscriptSegmentDetailDto>(
				TranscriptErrors.NotFound);

		var segmentId = TranscriptSegmentId.From(request.SegmentId);
		var segment = video.Transcript.TranscriptSegments.FirstOrDefault(s => s.Id == segmentId);
		if (segment is null)
			return Result.Failure<TranscriptSegmentDetailDto>(TranscriptSegmentErrors.NotFound);

		// Handle sequence change if provided
		if (request.Sequence.HasValue && request.Sequence.Value != segment.Sequence)
		{
			var updateSequenceResult = video.Transcript.UpdateSegmentSequence(segmentId, request.Sequence.Value);
			if (updateSequenceResult.IsFailure)
				return Result.Failure<TranscriptSegmentDetailDto>(updateSequenceResult.Error);
		}

		// Update segment content
		var updateResult = segment.Update(
			request.Text,
			request.StartTime,
			request.EndTime,
			request.Sequence ?? segment.Sequence,
			DateTimeOffset.UtcNow);

		if (updateResult.IsFailure)
			return Result.Failure<TranscriptSegmentDetailDto>(updateResult.Error);

		return Result.Success(TranscriptSegmentDetailDto.FromEntity(segment));
	}
}
