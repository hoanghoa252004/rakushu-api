using MediatR;
using Rakushu.Application.Usecases.Material.Transcript;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Shadowing.Speaker;

namespace Rakushu.Application.Usecases.Material.TranscriptSegment.CreateSegment;

internal sealed class CreateTranscriptSegmentHandler : IRequestHandler<CreateTranscriptSegmentCommand, Result<Guid>>
{
	private readonly IVideoRepository _videoRepository;
	private readonly ISpeakerRepository _speakerRepository;
	private readonly IUnitOfWork _unitOfWork;

	public CreateTranscriptSegmentHandler(
		IVideoRepository videoRepository,
		ISpeakerRepository speakerRepository,
		IUnitOfWork unitOfWork)
	{
		_videoRepository = videoRepository;
		_speakerRepository = speakerRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<Guid>> Handle(
		CreateTranscriptSegmentCommand request,
		CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var video = await _videoRepository.GetByIdAsync(VideoId.From(request.VideoId), cancellationToken);
			if (video is null)
				return Result.Failure<Guid>(VideoErrors.NotFound);

			var transcript = video.Transcript;
			if (transcript is null)
				return Result.Failure<Guid>(TranscriptErrors.NotFound);

			// Validate the speaker exists in the Speaker table
			var speakerId = SpeakerId.From(request.SpeakerId);
			var speaker = await _speakerRepository.GetByIdAsync(speakerId, cancellationToken);
			if (speaker is null)
				return Result.Failure<Guid>(SpeakerErrors.NotFound);

			// Get or create the transcript speaker for this speaker
			var transcriptSpeaker = transcript.TranscriptSpeakers.FirstOrDefault(x => x.SpeakerId == speakerId);
			if (transcriptSpeaker is null)
			{
				var addSpeakerResult = transcript.AddSpeaker(speaker.Code, speakerId);
				if (addSpeakerResult.IsFailure)
					return Result.Failure<Guid>(addSpeakerResult.Error);

				transcriptSpeaker = addSpeakerResult.Value;
			}

			// Determine the actual sequence for the new segment
			var currentCount = transcript.TranscriptSegments.Count;
			var sequence = request.Sequence ?? (currentCount + 1);

			if (sequence < 1 || sequence > currentCount + 1)
				return Result.Failure<Guid>(TranscriptSegmentErrors.InvalidSequence);

			// If inserting in the middle, shift affected segments
			if (sequence <= currentCount)
			{
				var segmentsToShift = transcript.TranscriptSegments
					.Where(s => s.Sequence >= sequence)
					.OrderByDescending(s => s.Sequence)
					.ToList();

				foreach (var seg in segmentsToShift)
				{
					var updateResult = transcript.UpdateSegmentSequence(
						TranscriptSegmentId.From(seg.Id.Value),
						seg.Sequence + 1);

					if (updateResult.IsFailure)
						return Result.Failure<Guid>(updateResult.Error);
				}
			}

			var segmentResult = transcript.AddSegment(
				transcriptSpeaker.Id,
				request.Text,
				request.StartTime,
				request.EndTime,
				sequence,
				DateTimeOffset.UtcNow);

			if (segmentResult.IsFailure)
				return Result.Failure<Guid>(segmentResult.Error);

			var segment = segmentResult.Value;

			// Subtitle
			var subtitleResult = segment.AddSubtitle(request.Subtitle.LanguageCode, request.Subtitle.Text);
			if (subtitleResult.IsFailure)
				return Result.Failure<Guid>(subtitleResult.Error);

			// Transcript items
			foreach (var item in request.Items.OrderBy(x => x.Sequence))
			{
				var itemResult = segment.AddItem(
					item.Text,
					item.Type,
					item.StartTime,
					item.EndTime,
					item.Confidence,
					item.Sequence);

				if (itemResult.IsFailure)
					return Result.Failure<Guid>(itemResult.Error);
			}

			return Result.Success(segment.Id.Value);
		}, cancellationToken);
	}
}
