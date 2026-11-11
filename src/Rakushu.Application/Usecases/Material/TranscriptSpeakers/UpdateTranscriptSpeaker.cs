using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSpeaker;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Shadowing.Speaker;

namespace Rakushu.Application.Usecases.Material.TranscriptSpeakers;

public sealed record UpdateTranscriptSpeakerCommand(
	Guid VideoId,
	Guid TranscriptSpeakerId,
	Guid SpeakerId,
	string Label) : IRequest<Result>;

internal sealed class UpdateTranscriptSpeakerHandler(
	IVideoRepository videoRepository,
	ISpeakerRepository speakerRepository,
	IUnitOfWork unitOfWork)
	: IRequestHandler<UpdateTranscriptSpeakerCommand, Result>
{
	public async Task<Result> Handle(UpdateTranscriptSpeakerCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var video = await videoRepository.GetByIdAsync(VideoId.From(request.VideoId), cancellationToken);
			if (video is null)
				return Result.Failure(VideoErrors.NotFound);

			var transcript = video.Transcript;
			if (transcript is null)
				return Result.Failure(TranscriptErrors.NotFound);

			var id = TranscriptSpeakerId.From(request.TranscriptSpeakerId);
			var transcriptSpeaker = transcript.TranscriptSpeakers.FirstOrDefault(x => x.Id == id);
			if (transcriptSpeaker is null)
				return Result.Failure(TranscriptSpeakerErrors.NotFound);

			var speakerId = SpeakerId.From(request.SpeakerId);
			var speaker = await speakerRepository.GetByIdAsync(speakerId, cancellationToken);
			if (speaker is null)
				return Result.Failure(SpeakerErrors.NotFound);

			if (transcript.TranscriptSpeakers.Any(x => x.Id != id && x.SpeakerId == speakerId))
				return Result.Failure(TranscriptSpeakerErrors.SpeakerAlreadyAssigned);

			transcriptSpeaker.Update(request.Label.Trim(), speakerId);
			return Result.Success();
		}, cancellationToken);
	}
}
