using FluentValidation;
using MediatR;
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

			var version = await repository.GetNextVersionAsync(videoId, cancellationToken);
			var created = ShadowingMaterial.Create(MaterialType.Curated, null, videoId, version, DateTimeOffset.UtcNow);
			if (created.IsFailure)
				return Result.Failure<Guid>(created.Error);

			repository.Add(created.Value);
			return Result.Success(created.Value.Id.Value);
		}, cancellationToken);
	}
}
