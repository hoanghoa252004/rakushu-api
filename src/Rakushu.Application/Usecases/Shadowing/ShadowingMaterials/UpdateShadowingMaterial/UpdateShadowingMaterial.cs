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

namespace Rakushu.Application.Usecases.Shadowing.ShadowingMaterials.UpdateShadowingMaterial;

public sealed record UpdateShadowingMaterialCommand(
	Guid MaterialId,
	string Name,
	Guid StartTranscriptSegmentId,
	Guid EndTranscriptSegmentId,
	IReadOnlyList<Guid> SpeakerIds) : IRequest<Result>;

internal sealed class UpdateShadowingMaterialValidator : AbstractValidator<UpdateShadowingMaterialCommand>
{
	public UpdateShadowingMaterialValidator()
	{
		RuleFor(x => x.MaterialId).NotEmpty().WithMessage("MaterialId is required");
		RuleFor(x => x.Name).NotEmpty().MaximumLength(200).WithMessage("Name is required (max 200 characters)");
		RuleFor(x => x.StartTranscriptSegmentId).NotEmpty().WithMessage("StartTranscriptSegmentId is required");
		RuleFor(x => x.EndTranscriptSegmentId).NotEmpty().WithMessage("EndTranscriptSegmentId is required");
		RuleFor(x => x.SpeakerIds).NotEmpty().WithMessage("SpeakerIds is required");
		RuleForEach(x => x.SpeakerIds).NotEmpty();
	}
}

internal sealed class UpdateShadowingMaterialHandler(
	IShadowingMaterialRepository repository,
	IShadowingRoleRepository roleRepository,
	IVideoRepository videoRepository,
	ICurrentUserContext currentUserContext,
	IUnitOfWork unitOfWork)
	: IRequestHandler<UpdateShadowingMaterialCommand, Result>
{
	public async Task<Result> Handle(UpdateShadowingMaterialCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var material = await repository.GetByIdAsync(ShadowingMaterialId.From(request.MaterialId), cancellationToken);
			if (material is null)
				return Result.Failure(ShadowingMaterialErrors.NotFound);

			var video = await videoRepository.GetByIdAsync(material.VideoId, cancellationToken);
			if (video is null)
				return Result.Failure(VideoErrors.NotFound);

			if (currentUserContext.Role == RoleCodes.Learner)
			{
				if (currentUserContext.UserId is null || material.CreatedById != currentUserContext.UserId)
					return Result.Failure(ShadowingMaterialErrors.LearnerNotAllowed);
			}
			else if (currentUserContext.Role != RoleCodes.LinguisticCurator)
				return Result.Failure(ShadowingMaterialErrors.LearnerNotAllowed);

			if (material.Status == ShadowingMaterialStatus.Archived)
				return Result.Failure(ShadowingMaterialErrors.NotEditable);

			var renamed = material.Rename(request.Name);
			if (renamed.IsFailure)
				return renamed;

			var startId = TranscriptSegmentId.From(request.StartTranscriptSegmentId);
			var endId = TranscriptSegmentId.From(request.EndTranscriptSegmentId);
			var existingSpeakerIds = (await roleRepository.GetByMaterialIdAsync(material.Id, cancellationToken))
				.Select(r => r.SpeakerId)
				.ToHashSet();

			var rangeChanged = material.StartTranscriptSegmentId != startId || material.EndTranscriptSegmentId != endId;

			var speakers = await ShadowingMaterialRangeValidator.ValidateAsync(
				repository, material.VideoId, request.StartTranscriptSegmentId, request.EndTranscriptSegmentId,
				request.SpeakerIds, material.Id, cancellationToken);
			if (speakers.IsFailure)
				return Result.Failure(speakers.Error);

			var speakersChanged = !existingSpeakerIds.SetEquals(speakers.Value);

			if (rangeChanged || speakersChanged)
			{
				if (await repository.HasSessionsAsync(material.Id, cancellationToken))
					return Result.Failure(ShadowingMaterialErrors.HasSessions);

				if (material.Status != ShadowingMaterialStatus.Processing)
				{
					if (currentUserContext.Role == RoleCodes.Learner
						&& currentUserContext.UserId is not null
						&& await repository.HasProcessingByLearnerAsync(currentUserContext.UserId, cancellationToken))
						return Result.Failure(ShadowingMaterialErrors.ProcessingMaterialExists);

					var changed = material.ChangeStatus(ShadowingMaterialStatus.Processing);
					if (changed.IsFailure)
						return changed;
				}

				material.SetRange(startId, endId);

				// Roles cascade-delete their lines; persist the removal before re-creating roles.
				foreach (var role in await roleRepository.GetByMaterialIdAsync(material.Id, cancellationToken))
					roleRepository.Delete(role);
				await unitOfWork.SaveChangesAsync(cancellationToken);

				var order = 1;
				foreach (var speakerId in speakers.Value)
				{
					var role = ShadowingRole.Create(material.Id, speakerId, order++);
					if (role.IsFailure)
						return Result.Failure(role.Error);
					roleRepository.Add(role.Value);
				}
			}

			repository.Update(material);
			return Result.Success();
		}, cancellationToken);
	}
}
