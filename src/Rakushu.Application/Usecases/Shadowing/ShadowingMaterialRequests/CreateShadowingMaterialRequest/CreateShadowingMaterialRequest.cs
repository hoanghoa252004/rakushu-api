using FluentValidation;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterialRequest;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingMaterialRequests.CreateShadowingMaterialRequest;

public sealed record CreateShadowingMaterialRequestCommand(Guid VideoId) : IRequest<Result<Guid>>;

internal sealed class CreateShadowingMaterialRequestValidator : AbstractValidator<CreateShadowingMaterialRequestCommand>
{
	public CreateShadowingMaterialRequestValidator()
	{
		RuleFor(x => x.VideoId).NotEmpty().WithMessage("VideoId is required");
	}
}

internal sealed class CreateShadowingMaterialRequestHandler(
	IShadowingMaterialRequestRepository repository,
	IVideoRepository videoRepository,
	ICurrentUserContext currentUser,
	IUnitOfWork unitOfWork)
	: IRequestHandler<CreateShadowingMaterialRequestCommand, Result<Guid>>
{
	public async Task<Result<Guid>> Handle(CreateShadowingMaterialRequestCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			if (currentUser.UserId is null)
				return Result.Failure<Guid>(ShadowingMaterialRequestErrors.Forbidden);

			var videoId = VideoId.From(request.VideoId);
			var video = await videoRepository.GetByIdAsync(videoId, cancellationToken);
			if (video is null)
				return Result.Failure<Guid>(VideoErrors.NotFound);

			if (video.Status != VideoStatus.Ready)
				return Result.Failure<Guid>(ShadowingMaterialRequestErrors.VideoNotReady);

			if (await repository.ExistsByVideoIdAsync(videoId, cancellationToken))
				return Result.Failure<Guid>(ShadowingMaterialRequestErrors.AlreadyExists);

			var code = await repository.GenerateNextCodeAsync(cancellationToken);
			var created = ShadowingMaterialRequest.Create(code, videoId, currentUser.UserId, DateTimeOffset.UtcNow);
			if (created.IsFailure)
				return Result.Failure<Guid>(created.Error);

			repository.Add(created.Value);
			return Result.Success(created.Value.Id.Value);
		}, cancellationToken);
	}
}
