using FluentValidation;
using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;
using Rakushu.Domain.Entities.Shadowing.ShadowingRoleLine;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingRoleLines.DeleteShadowingRoleLine;

public sealed record DeleteShadowingRoleLineCommand(Guid LineId) : IRequest<Result>;

internal sealed class DeleteShadowingRoleLineValidator : AbstractValidator<DeleteShadowingRoleLineCommand>
{
	public DeleteShadowingRoleLineValidator()
	{
		RuleFor(x => x.LineId).NotEmpty();
	}
}

internal sealed class DeleteShadowingRoleLineHandler(
	IShadowingRoleLineRepository repository,
	IShadowingMaterialRepository materialRepository,
	IUnitOfWork unitOfWork)
	: IRequestHandler<DeleteShadowingRoleLineCommand, Result>
{
	public async Task<Result> Handle(DeleteShadowingRoleLineCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var line = await repository.GetByIdAsync(ShadowingRoleLineId.From(request.LineId), cancellationToken);
			if (line is null)
				return Result.Failure(ShadowingRoleLineErrors.NotFound);

			var material = await materialRepository.GetByIdAsync(line.ShadowingMaterialId, cancellationToken);
			if (material is null)
				return Result.Failure(ShadowingMaterialErrors.NotFound);
			if (material.Status != ShadowingMaterialStatus.Reviewing)
				return Result.Failure(ShadowingMaterialErrors.NotEditable);

			repository.Delete(line);
			await unitOfWork.SaveChangesAsync(cancellationToken);
			await repository.RecomputeSortOrdersAsync(line.ShadowingMaterialId, cancellationToken);
			return Result.Success();
		}, cancellationToken);
	}
}
