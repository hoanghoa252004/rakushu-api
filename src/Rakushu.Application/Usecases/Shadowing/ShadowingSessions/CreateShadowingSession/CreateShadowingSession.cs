using FluentValidation;
using MediatR;
using Rakushu.Application.Common.Parsing;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;
using Rakushu.Domain.Entities.Shadowing.ShadowingSession;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingSessions.CreateShadowingSession;

public sealed record CreateShadowingSessionCommand(
	Guid MaterialId,
	string Title,
	string Visibility,
	DateTimeOffset ScheduledStartAt) : IRequest<Result<Guid>>;

internal sealed class CreateShadowingSessionValidator : AbstractValidator<CreateShadowingSessionCommand>
{
	public CreateShadowingSessionValidator()
	{
		RuleFor(x => x.MaterialId).NotEmpty();
		RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
		RuleFor(x => x.Visibility).NotEmpty();
	}
}

internal sealed class CreateShadowingSessionHandler(
	IShadowingSessionRepository repository,
	IShadowingMaterialRepository materialRepository,
	IVideoRepository videoRepository,
	ICurrentUserContext currentUser,
	IUnitOfWork unitOfWork)
	: IRequestHandler<CreateShadowingSessionCommand, Result<Guid>>
{
	public async Task<Result<Guid>> Handle(CreateShadowingSessionCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var visibility = EnumParser.Parse<ShadowingSessionVisibility>(request.Visibility, "visibility");
			if (visibility.IsFailure)
				return Result.Failure<Guid>(visibility.Error);

			var userId = currentUser.UserId;
			if (userId is null)
				return Result.Failure<Guid>(ShadowingSessionErrors.Unauthenticated);

			var material = await materialRepository.GetByIdAsync(
				ShadowingMaterialId.From(request.MaterialId), cancellationToken);
			if (material is null)
				return Result.Failure<Guid>(ShadowingSessionErrors.MaterialNotFound);
			if (material.Status != ShadowingMaterialStatus.Ready)
				return Result.Failure<Guid>(ShadowingSessionErrors.MaterialNotReady);

			var video = await videoRepository.GetByIdAsync(material.VideoId, cancellationToken);
			if (video is null)
				return Result.Failure<Guid>(VideoErrors.NotFound);

			if (material.CreatedById != userId)
				return Result.Failure<Guid>(ShadowingSessionErrors.MaterialNotAllowed);

			if (await repository.HasActiveHostedSessionAsync(userId, cancellationToken))
				return Result.Failure<Guid>(ShadowingSessionErrors.ActiveSessionExists);

			if (await repository.HasScheduleConflictAsync(
				userId, request.ScheduledStartAt, video.Duration, null, cancellationToken))
				return Result.Failure<Guid>(ShadowingSessionErrors.ScheduleConflict);

			var created = ShadowingSession.Create(
				material.Id, userId, request.Title, visibility.Value, request.ScheduledStartAt, DateTimeOffset.UtcNow);
			if (created.IsFailure)
				return Result.Failure<Guid>(created.Error);

			repository.Add(created.Value);
			return Result.Success(created.Value.Id.Value);
		}, cancellationToken);
	}
}
