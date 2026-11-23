using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSpeaker;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Shadowing.Speaker;

namespace Rakushu.Application.Usecases.Material.TranscriptSegment.UpdateSegmentSpeaker;

internal sealed class UpdateTranscriptSegmentSpeakerHandler
	: IRequestHandler<UpdateTranscriptSegmentSpeakerCommand, Result>
{
	private readonly IVideoRepository _videoRepository;
	private readonly ISpeakerRepository _speakerRepository;
	private readonly IUnitOfWork _unitOfWork;

	public UpdateTranscriptSegmentSpeakerHandler(
		IVideoRepository videoRepository,
		ISpeakerRepository speakerRepository,
		IUnitOfWork unitOfWork)
	{
		_videoRepository = videoRepository;
		_speakerRepository = speakerRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(
		UpdateTranscriptSegmentSpeakerCommand request,
		CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var video = await _videoRepository.GetByIdAsync(
				VideoId.From(request.VideoId),
				cancellationToken);

			if (video is null)
				return Result.Failure(VideoErrors.NotFound);

			var transcript = video.Transcript;
			if (transcript is null)
				return Result.Failure(TranscriptErrors.NotFound);

			if (video.Status == VideoStatus.Processing
				&& transcript.IsReviewed)
			{
				return Result.Failure(TranscriptErrors.CannotModifyReviewedTranscript);
			}

			var segmentId = TranscriptSegmentId.From(request.SegmentId);
			var segment = transcript.TranscriptSegments
				.FirstOrDefault(s => s.Id == segmentId);

			if (segment is null)
				return Result.Failure(TranscriptSegmentErrors.NotFound);

			if (request.TranscriptSpeakerId.HasValue)
			{
				var transcriptSpeakerId = TranscriptSpeakerId.From(request.TranscriptSpeakerId.Value);
				var targetTranscriptSpeaker = transcript.TranscriptSpeakers
					.FirstOrDefault(x => x.Id == transcriptSpeakerId);

				if (targetTranscriptSpeaker is null)
					return Result.Failure(TranscriptSpeakerErrors.NotFound);

				if (segment.TranscriptSpeakerId != targetTranscriptSpeaker.Id)
				{
					segment.SetSpeaker(targetTranscriptSpeaker.Id);
				}
			}
			else if (request.SpeakerId.HasValue)
			{
				var speakerId = SpeakerId.From(request.SpeakerId.Value);
				var speaker = await _speakerRepository.GetByIdAsync(
					speakerId,
					cancellationToken);

				if (speaker is null)
					return Result.Failure(SpeakerErrors.NotFound);

				var targetTranscriptSpeaker = transcript.TranscriptSpeakers
					.FirstOrDefault(x => x.SpeakerId == speakerId);

				if (targetTranscriptSpeaker is null)
					return Result.Failure(TranscriptSpeakerErrors.NotFound);

				if (segment.TranscriptSpeakerId != targetTranscriptSpeaker.Id)
				{
					segment.SetSpeaker(targetTranscriptSpeaker.Id);
				}
			}
			else
			{
				if (segment.TranscriptSpeakerId is not null)
				{
					segment.SetSpeaker(null);
				}
			}

			return Result.Success();
		}, cancellationToken);
	}
}
