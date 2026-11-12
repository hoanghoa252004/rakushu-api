using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSpeaker;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Application.Usecases.Material.TranscriptSpeakers;

public sealed record DeleteTranscriptSpeakerCommand(Guid VideoId, Guid TranscriptSpeakerId) : IRequest<Result>;

internal sealed class DeleteTranscriptSpeakerHandler(
	IVideoRepository videoRepository,
	IUnitOfWork unitOfWork)
	: IRequestHandler<DeleteTranscriptSpeakerCommand, Result>
{
	public async Task<Result> Handle(DeleteTranscriptSpeakerCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var video = await videoRepository.GetByIdAsync(VideoId.From(request.VideoId), cancellationToken);
			if (video is null)
				return Result.Failure(VideoErrors.NotFound);

			var transcript = video.Transcript;
			if (transcript is null)
				return Result.Failure(TranscriptErrors.NotFound);

			return transcript.RemoveSpeaker(TranscriptSpeakerId.From(request.TranscriptSpeakerId));
		}, cancellationToken);
	}
}
