using FluentValidation;
using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingMaterials.DeleteShadowingMaterial;

public sealed record DeleteShadowingMaterialCommand(Guid MaterialId) : IRequest<Result>;

internal sealed class DeleteShadowingMaterialValidator : AbstractValidator<DeleteShadowingMaterialCommand>
{
	public DeleteShadowingMaterialValidator()
	{
		RuleFor(x => x.MaterialId).NotEmpty().WithMessage("MaterialId is required");
	}
}

internal sealed class DeleteShadowingMaterialHandler(
	IShadowingMaterialRepository repository,
	IUnitOfWork unitOfWork)
	: IRequestHandler<DeleteShadowingMaterialCommand, Result>
{
	public async Task<Result> Handle(DeleteShadowingMaterialCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var material = await repository.GetByIdAsync(ShadowingMaterialId.From(request.MaterialId), cancellationToken);
			if (material is null)
				return Result.Failure(ShadowingMaterialErrors.NotFound);

			if (material.Status != ShadowingMaterialStatus.Processing || material.Status != ShadowingMaterialStatus.Reviewing)
				return Result.Failure(ShadowingMaterialErrors.CannotDelete);

			repository.Delete(material);
			return Result.Success();
		}, cancellationToken);
	}
}
