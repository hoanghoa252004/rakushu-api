using FluentValidation;
using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;
using Rakushu.Domain.Entities.Shadowing.ShadowingRole;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingRoles.DeleteShadowingRole;

public sealed record DeleteShadowingRoleCommand(Guid RoleId) : IRequest<Result>;

internal sealed class DeleteShadowingRoleValidator : AbstractValidator<DeleteShadowingRoleCommand>
{
	public DeleteShadowingRoleValidator()
	{
		RuleFor(x => x.RoleId).NotEmpty();
	}
}

internal sealed class DeleteShadowingRoleHandler(
	IShadowingRoleRepository repository,
	IShadowingMaterialRepository materialRepository,
	IUnitOfWork unitOfWork)
	: IRequestHandler<DeleteShadowingRoleCommand, Result>
{
	public async Task<Result> Handle(DeleteShadowingRoleCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var role = await repository.GetByIdAsync(ShadowingRoleId.From(request.RoleId), cancellationToken);
			if (role is null)
				return Result.Failure(ShadowingRoleErrors.NotFound);

			var material = await materialRepository.GetByIdAsync(role.ShadowingMaterialId, cancellationToken);
			if (material is null)
				return Result.Failure(ShadowingMaterialErrors.NotFound);
			if (material.Status != ShadowingMaterialStatus.Reviewing)
				return Result.Failure(ShadowingMaterialErrors.NotEditable);

			repository.Delete(role);
			return Result.Success();
		}, cancellationToken);
	}
}
