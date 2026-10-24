using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Shadowing.Speaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Shadowing.Speakers.GetSpeakerById;

internal sealed class GetSpeakerByIdHandler(ISpeakerRepository repository, IStorageService storageService)
	: IRequestHandler<GetSpeakerByIdQuery, Result<SpeakerDto>>
{
	public async Task<Result<SpeakerDto>> Handle(GetSpeakerByIdQuery request, CancellationToken cancellationToken)
	{
		var speaker = await repository.GetByIdAsync(SpeakerId.From(request.SpeakerId), cancellationToken);
		if (speaker is null)
			return Result.Failure<SpeakerDto>(SpeakerErrors.NotFound);

		return Result.Success(await SpeakerDto.FromEntityAsync(speaker, storageService, cancellationToken));
	}
}