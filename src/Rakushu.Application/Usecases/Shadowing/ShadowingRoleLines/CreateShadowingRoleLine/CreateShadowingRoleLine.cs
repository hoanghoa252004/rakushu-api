using FluentValidation;
using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;
using Rakushu.Domain.Entities.Shadowing.ShadowingRole;
using Rakushu.Domain.Entities.Shadowing.ShadowingRoleLine;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingRoleLines.CreateShadowingRoleLine;

public sealed record CreateShadowingRoleLineCommand(
	Guid MaterialId,
	Guid RoleId,
	int SortOrder,
	string Text,
	TimeSpan StartTime,
	TimeSpan EndTime,
	string AudioKey) : IRequest<Result<Guid>>;

internal sealed class CreateShadowingRoleLineValidator : AbstractValidator<CreateShadowingRoleLineCommand>
{
	public CreateShadowingRoleLineValidator()
	{
		RuleFor(x => x.MaterialId).NotEmpty();
		RuleFor(x => x.RoleId).NotEmpty();
		RuleFor(x => x.SortOrder).GreaterThan(0);
		RuleFor(x => x.Text).NotEmpty();
		RuleFor(x => x.StartTime).GreaterThanOrEqualTo(TimeSpan.Zero);
		RuleFor(x => x.EndTime).GreaterThan(x => x.StartTime).WithMessage("EndTime must be greater than StartTime");
		RuleFor(x => x.AudioKey).NotEmpty().MaximumLength(500);
	}
}

internal sealed class CreateShadowingRoleLineHandler(
	IShadowingRoleLineRepository repository,
	IShadowingRoleRepository roleRepository,
	IShadowingMaterialRepository materialRepository,
	IUnitOfWork unitOfWork)
	: IRequestHandler<CreateShadowingRoleLineCommand, Result<Guid>>
{
	public async Task<Result<Guid>> Handle(CreateShadowingRoleLineCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var materialId = ShadowingMaterialId.From(request.MaterialId);
			var material = await materialRepository.GetByIdAsync(materialId, cancellationToken);
			if (material is null)
				return Result.Failure<Guid>(ShadowingMaterialErrors.NotFound);
			if (material.Status != ShadowingMaterialStatus.Reviewing)
				return Result.Failure<Guid>(ShadowingMaterialErrors.NotEditable);

			var role = await roleRepository.GetByIdAsync(ShadowingRoleId.From(request.RoleId), cancellationToken);
			if (role is null)
				return Result.Failure<Guid>(ShadowingRoleErrors.NotFound);
			if (role.ShadowingMaterialId != materialId)
				return Result.Failure<Guid>(ShadowingRoleLineErrors.RoleNotInMaterial);

			if (await repository.SortOrderExistsAsync(materialId, request.SortOrder, null, cancellationToken))
				return Result.Failure<Guid>(ShadowingRoleLineErrors.SortOrderAlreadyExists);

			var created = ShadowingRoleLine.Create(
				materialId, role.Id, request.SortOrder, request.Text, request.StartTime, request.EndTime, request.AudioKey);
			if (created.IsFailure)
				return Result.Failure<Guid>(created.Error);

			repository.Add(created.Value);
			return Result.Success(created.Value.Id.Value);
		}, cancellationToken);
	}
}
