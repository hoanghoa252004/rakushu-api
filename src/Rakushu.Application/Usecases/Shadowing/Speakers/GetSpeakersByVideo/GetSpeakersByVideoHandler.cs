using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Shadowing.Speaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Shadowing.Speakers.GetSpeakersByVideo;

internal sealed class GetSpeakersByVideoHandler(
	ISpeakerRepository repository,
	IVideoRepository videoRepository,
	IStorageService storageService)
	: IRequestHandler<GetSpeakersByVideoQuery, Result<IReadOnlyList<SpeakerDto>>>
{
	public async Task<Result<IReadOnlyList<SpeakerDto>>> Handle(GetSpeakersByVideoQuery request, CancellationToken cancellationToken)
	{
		var videoId = VideoId.From(request.VideoId);
		var exists = (await videoRepository.FindAsync(v => v.Id == videoId, cancellationToken)).Any();
		if (!exists)
			return Result.Failure<IReadOnlyList<SpeakerDto>>(VideoErrors.NotFound);

		var speakers = await repository.GetByVideoIdAsync(videoId, cancellationToken);
		var dtos = await Task.WhenAll(speakers.Select(s => SpeakerDto.FromEntityAsync(s, storageService, cancellationToken)));

		return Result.Success<IReadOnlyList<SpeakerDto>>(dtos);
	}
}