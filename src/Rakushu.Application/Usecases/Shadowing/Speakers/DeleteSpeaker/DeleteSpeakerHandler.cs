using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Shadowing.Speaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Shadowing.Speakers.DeleteSpeaker;

internal sealed class DeleteSpeakerHandler(ISpeakerRepository repository, IUnitOfWork unitOfWork)
	: IRequestHandler<DeleteSpeakerCommand, Result>
{
	public async Task<Result> Handle(DeleteSpeakerCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var id = SpeakerId.From(request.SpeakerId);
			var speaker = await repository.GetByIdAsync(id, cancellationToken);
			if (speaker is null)
				return Result.Failure(SpeakerErrors.NotFound);

			if (await repository.IsUsedAsync(id, cancellationToken))
				return Result.Failure(SpeakerErrors.InUse);

			repository.Delete(speaker);
			return Result.Success();
		}, cancellationToken);
	}
}
