using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSpeaker;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Shadowing.Speaker;

namespace Rakushu.Application.Usecases.Material.TranscriptSpeakers;

public sealed record GetTranscriptSpeakersQuery(Guid VideoId) : IRequest<Result<IReadOnlyList<TranscriptSpeakerDto>>>;

public sealed record GetTranscriptSpeakerByIdQuery(Guid VideoId, Guid TranscriptSpeakerId) : IRequest<Result<TranscriptSpeakerDto>>;

internal sealed class GetTranscriptSpeakersHandler(
	IVideoRepository videoRepository,
	ISpeakerRepository speakerRepository,
	IStorageService storageService)
	: IRequestHandler<GetTranscriptSpeakersQuery, Result<IReadOnlyList<TranscriptSpeakerDto>>>
{
	public async Task<Result<IReadOnlyList<TranscriptSpeakerDto>>> Handle(
		GetTranscriptSpeakersQuery request,
		CancellationToken cancellationToken)
	{
		var video = await videoRepository.GetByIdAsync(VideoId.From(request.VideoId), cancellationToken);
		if (video is null)
			return Result.Failure<IReadOnlyList<TranscriptSpeakerDto>>(VideoErrors.NotFound);

		if (video.Transcript is null)
			return Result.Failure<IReadOnlyList<TranscriptSpeakerDto>>(TranscriptErrors.NotFound);

		var items = new List<TranscriptSpeakerDto>();
		foreach (var ts in video.Transcript.TranscriptSpeakers.OrderBy(x => x.Label))
			items.Add(await TranscriptSpeakerDto.FromEntityAsync(ts, speakerRepository, storageService, cancellationToken));

		return Result.Success<IReadOnlyList<TranscriptSpeakerDto>>(items);
	}
}

internal sealed class GetTranscriptSpeakerByIdHandler(
	IVideoRepository videoRepository,
	ISpeakerRepository speakerRepository,
	IStorageService storageService)
	: IRequestHandler<GetTranscriptSpeakerByIdQuery, Result<TranscriptSpeakerDto>>
{
	public async Task<Result<TranscriptSpeakerDto>> Handle(
		GetTranscriptSpeakerByIdQuery request,
		CancellationToken cancellationToken)
	{
		var video = await videoRepository.GetByIdAsync(VideoId.From(request.VideoId), cancellationToken);
		if (video is null)
			return Result.Failure<TranscriptSpeakerDto>(VideoErrors.NotFound);

		if (video.Transcript is null)
			return Result.Failure<TranscriptSpeakerDto>(TranscriptErrors.NotFound);

		var id = TranscriptSpeakerId.From(request.TranscriptSpeakerId);
		var ts = video.Transcript.TranscriptSpeakers.FirstOrDefault(x => x.Id == id);
		if (ts is null)
			return Result.Failure<TranscriptSpeakerDto>(TranscriptSpeakerErrors.NotFound);

		return Result.Success(await TranscriptSpeakerDto.FromEntityAsync(ts, speakerRepository, storageService, cancellationToken));
	}
}
