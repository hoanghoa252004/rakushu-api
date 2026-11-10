using FluentValidation;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;
using Rakushu.Domain.Entities.Shadowing.ShadowingRole;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingMaterials.CreateShadowingMaterial;

public sealed record CreateShadowingMaterialCommand(
	Guid VideoId,
	string Name,
	Guid StartTranscriptSegmentId,
	Guid EndTranscriptSegmentId,
	IReadOnlyList<Guid> SpeakerIds) : IRequest<Result<Guid>>;

internal sealed class CreateShadowingMaterialValidator : AbstractValidator<CreateShadowingMaterialCommand>
{
	public CreateShadowingMaterialValidator()
	{
		RuleFor(x => x.VideoId).NotEmpty().WithMessage("VideoId is required");
		RuleFor(x => x.Name).NotEmpty().MaximumLength(200).WithMessage("Name is required (max 200 characters)");
		RuleFor(x => x.StartTranscriptSegmentId).NotEmpty().WithMessage("StartTranscriptSegmentId is required");
		RuleFor(x => x.EndTranscriptSegmentId).NotEmpty().WithMessage("EndTranscriptSegmentId is required");
		RuleFor(x => x.SpeakerIds).NotEmpty().WithMessage("SpeakerIds is required");
		RuleForEach(x => x.SpeakerIds).NotEmpty();
	}
}

internal sealed class CreateShadowingMaterialHandler(
	IShadowingMaterialRepository repository,
	IShadowingRoleRepository roleRepository,
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

			if (currentUserContext.Role == RoleCodes.Learner)
			{
				if (currentUserContext.UserId is null
					|| (video.SourceType != VideoSource.Curated && video.LearnerId != currentUserContext.UserId))
					return Result.Failure<Guid>(ShadowingMaterialErrors.LearnerNotAllowed);

				if (await repository.HasProcessingByLearnerAsync(currentUserContext.UserId, cancellationToken))
					return Result.Failure<Guid>(ShadowingMaterialErrors.ProcessingMaterialExists);
			}
			else if (currentUserContext.Role != RoleCodes.LinguisticCurator)
				return Result.Failure<Guid>(ShadowingMaterialErrors.LearnerNotAllowed);

			var speakers = await ShadowingMaterialRangeValidator.ValidateAsync(
				repository, videoId, request.StartTranscriptSegmentId, request.EndTranscriptSegmentId,
				request.SpeakerIds, null, cancellationToken);
			if (speakers.IsFailure)
				return Result.Failure<Guid>(speakers.Error);

			var version = await repository.GetNextVersionAsync(videoId, cancellationToken);
			var created = ShadowingMaterial.Create(
				request.Name,
				TranscriptSegmentId.From(request.StartTranscriptSegmentId),
				TranscriptSegmentId.From(request.EndTranscriptSegmentId),
				videoId, version, currentUserContext.UserId!, DateTimeOffset.UtcNow);
			if (created.IsFailure)
				return Result.Failure<Guid>(created.Error);

			repository.Add(created.Value);

			var order = 1;
			foreach (var speakerId in speakers.Value)
			{
				var role = ShadowingRole.Create(created.Value.Id, speakerId, order++);
				if (role.IsFailure)
					return Result.Failure<Guid>(role.Error);
				roleRepository.Add(role.Value);
			}

			return Result.Success(created.Value.Id.Value);
		}, cancellationToken);
	}
}
