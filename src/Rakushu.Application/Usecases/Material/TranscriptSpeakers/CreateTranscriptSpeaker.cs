using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSpeaker;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Shadowing.Speaker;

namespace Rakushu.Application.Usecases.Material.TranscriptSpeakers;

public sealed record CreateTranscriptSpeakerCommand(
	Guid VideoId,
	Guid SpeakerId,
	string Label) : IRequest<Result<Guid>>;

internal sealed class CreateTranscriptSpeakerHandler(
	IVideoRepository videoRepository,
	ISpeakerRepository speakerRepository,
	IUnitOfWork unitOfWork)
	: IRequestHandler<CreateTranscriptSpeakerCommand, Result<Guid>>
{
	public async Task<Result<Guid>> Handle(CreateTranscriptSpeakerCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var video = await videoRepository.GetByIdAsync(VideoId.From(request.VideoId), cancellationToken);
			if (video is null)
				return Result.Failure<Guid>(VideoErrors.NotFound);

			var transcript = video.Transcript;
			if (transcript is null)
				return Result.Failure<Guid>(TranscriptErrors.NotFound);

			var speakerId = SpeakerId.From(request.SpeakerId);
			var speaker = await speakerRepository.GetByIdAsync(speakerId, cancellationToken);
			if (speaker is null)
				return Result.Failure<Guid>(SpeakerErrors.NotFound);

			if (transcript.TranscriptSpeakers.Any(x => x.SpeakerId == speakerId))
				return Result.Failure<Guid>(TranscriptSpeakerErrors.SpeakerAlreadyAssigned);

			var result = transcript.AddSpeaker(request.Label.Trim(), speakerId);
			if (result.IsFailure)
				return Result.Failure<Guid>(result.Error);

			return Result.Success(result.Value.Id.Value);
		}, cancellationToken);
	}
}
