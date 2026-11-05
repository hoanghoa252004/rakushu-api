using FluentValidation;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.AudioClips;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;
using Rakushu.Domain.Entities.Shadowing.ShadowingRole;
using Rakushu.Domain.Entities.Shadowing.ShadowingRoleLine;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingRoleLines.CreateShadowingRoleLine;

public sealed record CreateShadowingRoleLineCommand(
	Guid RoleId,
	IReadOnlyList<Guid> TranscriptSegmentIds) : IRequest<Result<Guid>>;

internal sealed class CreateShadowingRoleLineValidator : AbstractValidator<CreateShadowingRoleLineCommand>
{
	public CreateShadowingRoleLineValidator()
	{
		RuleFor(x => x.RoleId).NotEmpty();
		RuleFor(x => x.TranscriptSegmentIds).NotEmpty().WithMessage("At least one transcript segment is required");
		RuleForEach(x => x.TranscriptSegmentIds).NotEmpty();
	}
}

internal sealed class CreateShadowingRoleLineHandler(
	IShadowingRoleLineRepository repository,
	IShadowingRoleRepository roleRepository,
	IShadowingMaterialRepository materialRepository,
	IAudioClipService audioClipService,
	IUnitOfWork unitOfWork)
	: IRequestHandler<CreateShadowingRoleLineCommand, Result<Guid>>
{
	public async Task<Result<Guid>> Handle(CreateShadowingRoleLineCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var role = await roleRepository.GetByIdAsync(ShadowingRoleId.From(request.RoleId), cancellationToken);
			if (role is null)
				return Result.Failure<Guid>(ShadowingRoleErrors.NotFound);

			var materialId = role.ShadowingMaterialId;
			var material = await materialRepository.GetByIdAsync(materialId, cancellationToken);
			if (material is null)
				return Result.Failure<Guid>(ShadowingMaterialErrors.NotFound);
			if (material.Status != ShadowingMaterialStatus.Reviewing)
				return Result.Failure<Guid>(ShadowingMaterialErrors.NotEditable);

			var resolved = await RoleLineSegmentResolver.ResolveAsync(
				repository, material, request.TranscriptSegmentIds, null, cancellationToken);
			if (resolved.IsFailure)
				return Result.Failure<Guid>(resolved.Error);

			var sourceKey = await materialRepository.GetVideoSourceKeyAsync(material.VideoId, cancellationToken);
			if (string.IsNullOrWhiteSpace(sourceKey))
				return Result.Failure<Guid>(ShadowingMaterialErrors.VideoNotReady);

			var audioKey = await audioClipService.ExtractClipAsync(
				sourceKey,
				$"shadowing/{material.Id.Value}/lines/{Guid.NewGuid()}",
				resolved.Value.StartTime,
				resolved.Value.EndTime,
				cancellationToken);

			var created = ShadowingRoleLine.Create(
				materialId, role.Id, 1, resolved.Value.Text, resolved.Value.StartTime, resolved.Value.EndTime, audioKey);
			if (created.IsFailure)
				return Result.Failure<Guid>(created.Error);

			created.Value.SetSegments(resolved.Value.Ids);
			repository.Add(created.Value);
			await unitOfWork.SaveChangesAsync(cancellationToken);
			await repository.RecomputeSortOrdersAsync(materialId, cancellationToken);
			return Result.Success(created.Value.Id.Value);
		}, cancellationToken);
	}
}
