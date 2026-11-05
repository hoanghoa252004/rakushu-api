using FluentValidation;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingMaterials.CreateShadowingMaterial;

public sealed record CreateShadowingMaterialCommand(Guid VideoId) : IRequest<Result<Guid>>;

internal sealed class CreateShadowingMaterialValidator : AbstractValidator<CreateShadowingMaterialCommand>
{
	public CreateShadowingMaterialValidator()
	{
		RuleFor(x => x.VideoId).NotEmpty().WithMessage("VideoId is required");
	}
}

internal sealed class CreateShadowingMaterialHandler(
	IShadowingMaterialRepository repository,
	IVideoRepository videoRepository,
	ICurrentUserContext currentUserContext,
	IUnitOfWork unitOfWork)
	: IRequestHandler<CreateShadowingMaterialCommand, Result<Guid>>
{
	public async Task<Result<Guid>> Handle(CreateShadowingMaterialCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var videoId = VideoId.From(request.VideoId);
			var video = await videoRepository.GetByIdAsync(videoId, cancellationToken);
			if (video is null)
				return Result.Failure<Guid>(VideoErrors.NotFound);

			if (video.Status != VideoStatus.Ready)
				return Result.Failure<Guid>(ShadowingMaterialErrors.VideoNotReady);

			var type = MaterialType.Curated;
			if (currentUserContext.Role == RoleCodes.Learner)
			{
				if (currentUserContext.UserId is null || video.LearnerId != currentUserContext.UserId)
					return Result.Failure<Guid>(ShadowingMaterialErrors.LearnerNotAllowed);

				if (await repository.ExistsByVideoAndTypeAsync(videoId, MaterialType.RequestedByLearner, cancellationToken))
					return Result.Failure<Guid>(ShadowingMaterialErrors.LearnerMaterialAlreadyExists);

				type = MaterialType.RequestedByLearner;
			}
			else if (currentUserContext.Role != RoleCodes.LinguisticCurator)
				return Result.Failure<Guid>(ShadowingMaterialErrors.LearnerNotAllowed);

			var version = await repository.GetNextVersionAsync(videoId, cancellationToken);
			var created = ShadowingMaterial.Create(type, videoId, version, DateTimeOffset.UtcNow);
			if (created.IsFailure)
				return Result.Failure<Guid>(created.Error);

			repository.Add(created.Value);
			return Result.Success(created.Value.Id.Value);
		}, cancellationToken);
	}
}
