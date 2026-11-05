using FluentValidation;
using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;
using Rakushu.Domain.Entities.Shadowing.ShadowingRole;
using Rakushu.Domain.Entities.Shadowing.ShadowingRoleLine;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingRoleLines.UpdateShadowingRoleLine;

public sealed record UpdateShadowingRoleLineCommand(
	Guid LineId,
	Guid RoleId,
	int SortOrder,
	string Text,
	TimeSpan StartTime,
	TimeSpan EndTime,
	string AudioKey) : IRequest<Result>;

internal sealed class UpdateShadowingRoleLineValidator : AbstractValidator<UpdateShadowingRoleLineCommand>
{
	public UpdateShadowingRoleLineValidator()
	{
		RuleFor(x => x.LineId).NotEmpty();
		RuleFor(x => x.RoleId).NotEmpty();
		RuleFor(x => x.SortOrder).GreaterThan(0);
		RuleFor(x => x.Text).NotEmpty();
		RuleFor(x => x.StartTime).GreaterThanOrEqualTo(TimeSpan.Zero);
		RuleFor(x => x.EndTime).GreaterThan(x => x.StartTime).WithMessage("EndTime must be greater than StartTime");
		RuleFor(x => x.AudioKey).NotEmpty().MaximumLength(500);
	}
}

internal sealed class UpdateShadowingRoleLineHandler(
	IShadowingRoleLineRepository repository,
	IShadowingRoleRepository roleRepository,
	IShadowingMaterialRepository materialRepository,
	IUnitOfWork unitOfWork)
	: IRequestHandler<UpdateShadowingRoleLineCommand, Result>
{
	public async Task<Result> Handle(UpdateShadowingRoleLineCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var lineId = ShadowingRoleLineId.From(request.LineId);
			var line = await repository.GetByIdAsync(lineId, cancellationToken);
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

			if (await repository.SortOrderExistsAsync(line.ShadowingMaterialId, request.SortOrder, lineId, cancellationToken))
				return Result.Failure(ShadowingRoleLineErrors.SortOrderAlreadyExists);

			var updated = line.Update(
				role.Id, request.SortOrder, request.Text, request.StartTime, request.EndTime, request.AudioKey);
			if (updated.IsFailure)
				return updated;

			repository.Update(line);
			return Result.Success();
		}, cancellationToken);
	}
}
