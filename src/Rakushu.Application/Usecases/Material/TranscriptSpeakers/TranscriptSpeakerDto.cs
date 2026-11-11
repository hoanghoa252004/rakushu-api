using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Application.Usecases.Shadowing.Speakers;
using Rakushu.Domain.Entities.Shadowing.Speaker;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSpeaker;

namespace Rakushu.Application.Usecases.Material.TranscriptSpeakers;

public sealed record TranscriptSpeakerDto(
	Guid Id,
	Guid TranscriptId,
	string Label,
	Guid? SpeakerId,
	SpeakerDto? Speaker)
{
	public static async Task<TranscriptSpeakerDto> FromEntityAsync(
		TranscriptSpeaker transcriptSpeaker,
		ISpeakerRepository speakerRepository,
		IStorageService storageService,
		CancellationToken cancellationToken)
	{
		SpeakerDto? speakerDto = null;
		if (transcriptSpeaker.SpeakerId is not null)
		{
			var speaker = await speakerRepository.GetByIdAsync(transcriptSpeaker.SpeakerId, cancellationToken);
			if (speaker is not null)
				speakerDto = await SpeakerDto.FromEntityAsync(speaker, storageService, cancellationToken);
		}

		return new TranscriptSpeakerDto(
			transcriptSpeaker.Id.Value,
			transcriptSpeaker.TranscriptId.Value,
			transcriptSpeaker.Label,
			transcriptSpeaker.SpeakerId?.Value,
			speakerDto);
	}
}
