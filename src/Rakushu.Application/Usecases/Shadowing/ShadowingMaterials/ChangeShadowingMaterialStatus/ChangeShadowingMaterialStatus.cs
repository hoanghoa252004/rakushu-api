using FluentValidation;
using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;
using Rakushu.Domain.Entities.Shadowing.ShadowingRole;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingMaterials.ChangeShadowingMaterialStatus;

public sealed record ChangeShadowingMaterialStatusCommand(Guid MaterialId, ShadowingMaterialStatus Status)
	: IRequest<Result>;

internal sealed class ChangeShadowingMaterialStatusValidator : AbstractValidator<ChangeShadowingMaterialStatusCommand>
{
	public ChangeShadowingMaterialStatusValidator()
	{
		RuleFor(x => x.MaterialId).NotEmpty().WithMessage("MaterialId is required");
		RuleFor(x => x.Status).IsInEnum().WithMessage("Invalid status");
	}
}

internal sealed class ChangeShadowingMaterialStatusHandler(
	IShadowingMaterialRepository repository,
	IShadowingRoleRepository roleRepository,
	IUnitOfWork unitOfWork)
	: IRequestHandler<ChangeShadowingMaterialStatusCommand, Result>
{
	public async Task<Result> Handle(ChangeShadowingMaterialStatusCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var material = await repository.GetByIdAsync(ShadowingMaterialId.From(request.MaterialId), cancellationToken);
			if (material is null)
				return Result.Failure(ShadowingMaterialErrors.NotFound);

			if (request.Status == ShadowingMaterialStatus.Ready
				&& await roleRepository.HasRoleWithoutLinesAsync(material.Id, cancellationToken))
				return Result.Failure(ShadowingMaterialErrors.RolesMissingLines);

			var changed = material.ChangeStatus(request.Status);
			if (changed.IsFailure)
				return changed;

			return Result.Success();
		}, cancellationToken);
	}
}
