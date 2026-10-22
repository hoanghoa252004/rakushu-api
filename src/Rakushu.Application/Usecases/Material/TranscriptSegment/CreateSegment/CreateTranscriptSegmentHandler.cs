using MediatR;
using Rakushu.Application.Usecases.Material.Transcript;
using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSpeaker;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Application.Usecases.Material.TranscriptSegment.CreateSegment;

internal sealed class CreateTranscriptSegmentHandler : IRequestHandler<CreateTranscriptSegmentCommand, Result<Guid>>
{
	private readonly IVideoRepository _videoRepository;

	public CreateTranscriptSegmentHandler(IVideoRepository videoRepository)
	{
		_videoRepository = videoRepository;
	}

	public async Task<Result<Guid>> Handle(
		CreateTranscriptSegmentCommand request,
		CancellationToken cancellationToken)
	{
		var video = await _videoRepository.GetByIdAsync(VideoId.From(request.VideoId), cancellationToken);
		if (video is null)
			return Result.Failure<Guid>(VideoErrors.NotFound);

		if (video.Transcript is null)
			return Result.Failure<Guid>(
				TranscriptErrors.NotFound);

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
					return Result.Failure<Guid>(updateResult.Error);
			}
		}

		var segmentResult = video.Transcript.AddSegment(
			TranscriptSpeakerId.From(request.SpeakerId),
			request.Text,
			request.StartTime,
			request.EndTime,
			sequence,
			DateTimeOffset.UtcNow);

		if (segmentResult.IsFailure)
			return Result.Failure<Guid>(segmentResult.Error);

		var segment = segmentResult.Value;

		return Result.Success(segment.Id.Value);
	}
}
