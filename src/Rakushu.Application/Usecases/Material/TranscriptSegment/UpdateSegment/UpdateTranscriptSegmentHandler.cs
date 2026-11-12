using MediatR;
using Rakushu.Application.Usecases.Material.Transcript;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSpeaker;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Shadowing.Speaker;

namespace Rakushu.Application.Usecases.Material.TranscriptSegment.UpdateSegment;

internal sealed class UpdateTranscriptSegmentHandler
	: IRequestHandler<UpdateTranscriptSegmentCommand, Result>
{
	private readonly IVideoRepository _videoRepository;
	private readonly ISpeakerRepository _speakerRepository;
	private readonly ISegmentNlpService _segmentNlpService;
	private readonly IUnitOfWork _unitOfWork;

	public UpdateTranscriptSegmentHandler(
		IVideoRepository videoRepository,
		ISpeakerRepository speakerRepository,
		ISegmentNlpService segmentNlpService,
		IUnitOfWork unitOfWork)
	{
		_videoRepository = videoRepository;
		_speakerRepository = speakerRepository;
		_segmentNlpService = segmentNlpService;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(
		UpdateTranscriptSegmentCommand request,
		CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			// 1. Get video
			var video = await _videoRepository.GetByIdAsync(
				VideoId.From(request.VideoId),
				cancellationToken);

			if (video is null)
				return Result.Failure(VideoErrors.NotFound);

			// 2. Get transcript
			var transcript = video.Transcript;

			if (transcript is null)
				return Result.Failure(TranscriptErrors.NotFound);

			// 3. Validate transcript modification
			if (video.Status == VideoStatus.Processing
				&& transcript.IsReviewed)
			{
				return Result.Failure(
					TranscriptErrors.CannotModifyReviewedTranscript);
			}

			// 4. Get segment
			var segmentId = TranscriptSegmentId.From(request.SegmentId);

			var segment = transcript.TranscriptSegments
				.FirstOrDefault(s => s.Id == segmentId);

			if (segment is null)
				return Result.Failure(TranscriptSegmentErrors.NotFound);

			// 5. Validate speaker (optional)
			if (request.SpeakerId is not null)
			{
				var speakerId = SpeakerId.From(request.SpeakerId.Value);

				var speaker = await _speakerRepository.GetByIdAsync(
					speakerId,
					cancellationToken);

				if (speaker is null)
					return Result.Failure(SpeakerErrors.NotFound);

				var segmentSpeaker = segment.TranscriptSpeakerId is null
					? null
					: transcript.TranscriptSpeakers.FirstOrDefault(
						x => x.Id == segment.TranscriptSpeakerId);

				if (segmentSpeaker is null)
					return Result.Failure(TranscriptSpeakerErrors.NotFound);

				if (segmentSpeaker.SpeakerId != speakerId)
				{
					if (transcript.TranscriptSpeakers.Any(
						x => x.Id != segmentSpeaker.Id
							 && x.SpeakerId == speakerId))
					{
						return Result.Failure(
							TranscriptSpeakerErrors.SpeakerAlreadyAssigned);
					}

					segmentSpeaker.SetSpeakerId(speakerId);
				}
			}

			// 6. Check whether text has changed
			var textChanged = !string.Equals(
				segment.Text,
				request.Text,
				StringComparison.Ordinal);

			// 7. Update sequence
			if (request.Sequence.HasValue
				&& request.Sequence.Value != segment.Sequence)
			{
				var seqResult = transcript.UpdateSegmentSequence(
					segmentId,
					request.Sequence.Value);

				if (seqResult.IsFailure)
					return Result.Failure(seqResult.Error);
			}

			// 8. Update segment
			var updateResult = segment.Update(
				request.Text,
				request.StartTime,
				request.EndTime,
				segment.Sequence,
				DateTimeOffset.UtcNow);

			if (updateResult.IsFailure)
				return Result.Failure(updateResult.Error);

			// 9. Update subtitle (optional)
			if (request.Subtitle is not null)
			{
				var subtitleResult = segment.UpsertSubtitle(
					request.Subtitle.LanguageCode,
					request.Subtitle.Text);

				if (subtitleResult.IsFailure)
					return Result.Failure(subtitleResult.Error);
			}

			// 10. Replace transcript items
			segment.ClearItems();

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
					return Result.Failure(itemResult.Error);
			}

			// 11. Re-run NLP only when text has changed
			if (textChanged)
			{
				await _segmentNlpService.ReplaceAsync(
					segment,
					cancellationToken);
			}

			return Result.Success();

		}, cancellationToken);
	}
}