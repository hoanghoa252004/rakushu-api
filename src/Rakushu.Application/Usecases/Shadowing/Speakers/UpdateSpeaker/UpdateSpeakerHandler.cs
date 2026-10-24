using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Shadowing.Speaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Shadowing.Speakers.UpdateSpeaker;

internal sealed class UpdateSpeakerHandler(ISpeakerRepository repository, IUnitOfWork unitOfWork)
	: IRequestHandler<UpdateSpeakerCommand, Result>
{
	public async Task<Result> Handle(UpdateSpeakerCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var id = SpeakerId.From(request.SpeakerId);
			var speaker = await repository.GetByIdAsync(id, cancellationToken);
			if (speaker is null)
				return Result.Failure(SpeakerErrors.NotFound);

			return speaker.Update(
				request.Name,
				request.JapaneseName.Trim(),
				string.IsNullOrWhiteSpace(request.AvatarKey) ? null : request.AvatarKey,
				request.Description,
				request.ThemeColor,
				DateTimeOffset.UtcNow);
		}, cancellationToken);
	}
}