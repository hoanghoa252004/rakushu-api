using FluentValidation;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.AudioClips;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;
using Rakushu.Domain.Entities.Shadowing.ShadowingRole;
using Rakushu.Domain.Entities.Shadowing.ShadowingRoleLine;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingRoleLines.UpdateShadowingRoleLine;

public sealed record UpdateShadowingRoleLineCommand(
	Guid LineId,
	Guid RoleId,
	IReadOnlyList<Guid> TranscriptSegmentIds) : IRequest<Result>;

internal sealed class UpdateShadowingRoleLineValidator : AbstractValidator<UpdateShadowingRoleLineCommand>
{
	public UpdateShadowingRoleLineValidator()
	{
		RuleFor(x => x.LineId).NotEmpty();
		RuleFor(x => x.RoleId).NotEmpty();
		RuleFor(x => x.TranscriptSegmentIds).NotEmpty().WithMessage("At least one transcript segment is required");
		RuleForEach(x => x.TranscriptSegmentIds).NotEmpty();
	}
}

internal sealed class UpdateShadowingRoleLineHandler(
	IShadowingRoleLineRepository repository,
	IShadowingRoleRepository roleRepository,
	IShadowingMaterialRepository materialRepository,
	IAudioClipService audioClipService,
	IUnitOfWork unitOfWork)
	: IRequestHandler<UpdateShadowingRoleLineCommand, Result>
{
	public async Task<Result> Handle(UpdateShadowingRoleLineCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var lineId = ShadowingRoleLineId.From(request.LineId);
			var line = await repository.GetByIdWithSegmentsAsync(lineId, cancellationToken);
			if (line is null)
				return Result.Failure(ShadowingRoleLineErrors.NotFound);

			var material = await materialRepository.GetByIdAsync(line.ShadowingMaterialId, cancellationToken);
			if (material is null)
				return Result.Failure(ShadowingMaterialErrors.NotFound);
			if (material.Status != ShadowingMaterialStatus.Reviewing)
				return Result.Failure(ShadowingMaterialErrors.NotEditable);

			var role = await roleRepository.GetByIdAsync(ShadowingRoleId.From(request.RoleId), cancellationToken);
			if (role is null)
				return Result.Failure(ShadowingRoleErrors.NotFound);
			if (role.ShadowingMaterialId != line.ShadowingMaterialId)
				return Result.Failure(ShadowingRoleLineErrors.RoleNotInMaterial);

			var resolved = await RoleLineSegmentResolver.ResolveAsync(
				repository, material, request.TranscriptSegmentIds, lineId, cancellationToken);
			if (resolved.IsFailure)
				return Result.Failure(resolved.Error);

			var audioKey = line.AudioKey;
			var oldIds = line.TranscriptSegments.Select(x => x.TranscriptSegmentId.Value).ToHashSet();
			if (!oldIds.SetEquals(resolved.Value.Ids.Select(x => x.Value)))
			{
				var sourceKey = await materialRepository.GetVideoSourceKeyAsync(material.VideoId, cancellationToken);
				if (string.IsNullOrWhiteSpace(sourceKey))
					return Result.Failure(ShadowingMaterialErrors.VideoNotReady);

				audioKey = await audioClipService.ExtractClipAsync(
					sourceKey,
					$"shadowing/{material.Id.Value}/lines/{Guid.NewGuid()}",
					resolved.Value.StartTime,
					resolved.Value.EndTime,
					cancellationToken);
			}

			var updated = line.Update(
				role.Id, line.SortOrder, resolved.Value.Text, resolved.Value.StartTime, resolved.Value.EndTime, audioKey);
			if (updated.IsFailure)
				return updated;

			line.SetSegments(resolved.Value.Ids);
			repository.Update(line);
			await unitOfWork.SaveChangesAsync(cancellationToken);
			await repository.RecomputeSortOrdersAsync(line.ShadowingMaterialId, cancellationToken);
			return Result.Success();
		}, cancellationToken);
	}
}
