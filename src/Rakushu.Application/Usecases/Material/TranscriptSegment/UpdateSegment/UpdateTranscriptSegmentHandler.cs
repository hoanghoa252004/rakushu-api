using MediatR;
using Rakushu.Application.Usecases.Material.Transcript;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Shadowing.Speaker;

namespace Rakushu.Application.Usecases.Material.TranscriptSegment.UpdateSegment;

internal sealed class UpdateTranscriptSegmentHandler : IRequestHandler<UpdateTranscriptSegmentCommand, Result>
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
			var video = await _videoRepository.GetByIdAsync(VideoId.From(request.VideoId), cancellationToken);
			if (video is null)
				return Result.Failure(VideoErrors.NotFound);

			var transcript = video.Transcript;
			if (transcript is null)
				return Result.Failure(TranscriptErrors.NotFound);

			var segmentId = TranscriptSegmentId.From(request.SegmentId);
			var segment = transcript.TranscriptSegments.FirstOrDefault(s => s.Id == segmentId);
			if (segment is null)
				return Result.Failure(TranscriptSegmentErrors.NotFound);

			// Validate speaker
			var speakerId = SpeakerId.From(request.SpeakerId);
			var speaker = await _speakerRepository.GetByIdAsync(speakerId, cancellationToken);
			if (speaker is null)
				return Result.Failure(SpeakerErrors.NotFound);

			var transcriptSpeaker = transcript.TranscriptSpeakers.FirstOrDefault(x => x.SpeakerId == speakerId);
			if (transcriptSpeaker is null)
			{
				var addSpeakerResult = transcript.AddSpeaker(speaker.Code, speakerId);
				if (addSpeakerResult.IsFailure)
					return Result.Failure(addSpeakerResult.Error);

				transcriptSpeaker = addSpeakerResult.Value;
			}

			// Sequence
			if (request.Sequence.HasValue && request.Sequence.Value != segment.Sequence)
			{
				var seqResult = transcript.UpdateSegmentSequence(segmentId, request.Sequence.Value);
				if (seqResult.IsFailure)
					return Result.Failure(seqResult.Error);
			}

			var textChanged = !string.Equals(segment.Text, request.Text, StringComparison.Ordinal);

			var updateResult = segment.Update(
				request.Text,
				request.StartTime,
				request.EndTime,
				segment.Sequence,
				DateTimeOffset.UtcNow);

			if (updateResult.IsFailure)
				return Result.Failure(updateResult.Error);

			segment.SetSpeaker(transcriptSpeaker.Id);

			// Subtitle
			var subtitleResult = segment.UpsertSubtitle(request.Subtitle.LanguageCode, request.Subtitle.Text);
			if (subtitleResult.IsFailure)
				return Result.Failure(subtitleResult.Error);

			// Items: full replacement
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

			// Text changed: re-run NLP, replacing old bunsetsus and tokens
			if (textChanged)
				await _segmentNlpService.ReplaceAsync(segment, cancellationToken);

			return Result.Success();
		}, cancellationToken);
	}
}
