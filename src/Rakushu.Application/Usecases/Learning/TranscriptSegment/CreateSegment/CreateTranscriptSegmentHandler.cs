using MediatR;
using Rakushu.Application.Usecases.Learning.Transcript;
using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Video;
using Rakushu.Domain.Entities.Video.Transcript.TranscriptSegment;

namespace Rakushu.Application.Usecases.Learning.TranscriptSegment.CreateSegment;

internal sealed class CreateTranscriptSegmentHandler : IRequestHandler<CreateTranscriptSegmentCommand, Result<TranscriptSegmentDetailDto>>
	{
		private readonly IVideoRepository _videoRepository;

		public CreateTranscriptSegmentHandler(IVideoRepository videoRepository)
		{
			_videoRepository = videoRepository;
		}

	public async Task<Result<TranscriptSegmentDetailDto>> Handle(
		CreateTranscriptSegmentCommand request,
		CancellationToken cancellationToken)
	{
		var video = await _videoRepository.GetByIdAsync(VideoId.From(request.VideoId), cancellationToken);
		if (video is null)
			return Result.Failure<TranscriptSegmentDetailDto>(VideoErrors.NotFound);

		if (video.Transcript is null)
			return Result.Failure<TranscriptSegmentDetailDto>(
				Domain.Entities.Video.Transcript.TranscriptErrors.NotFound);

		// Determine the actual sequence for the new segment
		var currentMaxSequence = video.Transcript.TranscriptSegments.Count;
		var sequence = request.Sequence ?? (currentMaxSequence + 1);

		// If inserting at a specific sequence, shift affected segments
		if (request.Sequence.HasValue && sequence > 1 && sequence <= currentMaxSequence)
		{
			// Increment sequences for all segments at or after the insertion point
			var segmentsToShift = video.Transcript.TranscriptSegments
				.Where(s => s.Sequence >= sequence)
				.OrderByDescending(s => s.Sequence)
				.ToList();

			foreach (var seg in segmentsToShift)
			{
				var updateResult = video.Transcript.UpdateSegmentSequence(
					TranscriptSegmentId.From(seg.Id.Value),
					seg.Sequence + 1);

				if (updateResult.IsFailure)
					return Result.Failure<TranscriptSegmentDetailDto>(updateResult.Error);
			}
		}

		var segmentResult = video.Transcript.AddSegment(
			request.Text,
			request.StartTime,
			request.EndTime,
			sequence,
			DateTimeOffset.UtcNow,
			DateTimeOffset.UtcNow);

		if (segmentResult.IsFailure)
			return Result.Failure<TranscriptSegmentDetailDto>(segmentResult.Error);

		var segment = segmentResult.Value;

		return Result.Success(TranscriptSegmentDetailDto.FromEntity(segment));
	}
}
