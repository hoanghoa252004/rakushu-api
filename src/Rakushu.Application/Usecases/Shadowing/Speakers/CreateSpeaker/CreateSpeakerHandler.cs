using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Shadowing.Speaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Shadowing.Speakers.CreateSpeaker;


internal sealed class CreateSpeakerHandler(ISpeakerRepository repository, IUnitOfWork unitOfWork)
	: IRequestHandler<CreateSpeakerCommand, Result<Guid>>
{
	public async Task<Result<Guid>> Handle(CreateSpeakerCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var code = request.Code.Trim();
			if (await repository.CodeExistsAsync(code, null, cancellationToken))
				return Result.Failure<Guid>(SpeakerErrors.CodeAlreadyExists);

			var now = DateTimeOffset.UtcNow;
			var result = Speaker.Create(
				code,
				request.Name,
				request.JapaneseName.Trim(),
				string.IsNullOrWhiteSpace(request.AvatarKey) ? null : request.AvatarKey,
				request.Description,
				request.ThemeColor,
				now,
				now);

			if (result.IsFailure)
				return Result.Failure<Guid>(result.Error);

			repository.Add(result.Value);
			return Result.Success(result.Value.Id.Value);
		}, cancellationToken);
	}
}
