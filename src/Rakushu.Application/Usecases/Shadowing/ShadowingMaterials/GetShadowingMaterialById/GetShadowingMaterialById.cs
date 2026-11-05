using FluentValidation;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Domain.Entities.Shadowing.ShadowingRole;
using Rakushu.Domain.Entities.Shadowing.ShadowingRoleLine;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingMaterials.GetShadowingMaterialById;

public sealed record GetShadowingMaterialByIdQuery(
	Guid MaterialId,
	int LinePageNumber = 1,
	int LinePageSize = 20) : IRequest<Result<ShadowingMaterialDto>>;

internal sealed class GetShadowingMaterialByIdValidator : AbstractValidator<GetShadowingMaterialByIdQuery>
{
	public GetShadowingMaterialByIdValidator()
	{
		RuleFor(x => x.MaterialId).NotEmpty().WithMessage("MaterialId is required");
		RuleFor(x => x.LinePageNumber).GreaterThan(0);
		RuleFor(x => x.LinePageSize).InclusiveBetween(1, 100);
	}
}

internal sealed class GetShadowingMaterialByIdHandler(
	IShadowingMaterialRepository repository,
	IShadowingRoleRepository roleRepository,
	IShadowingRoleLineRepository roleLineRepository,
	IStorageService storageService,
	ICurrentUserContext currentUser)
	: IRequestHandler<GetShadowingMaterialByIdQuery, Result<ShadowingMaterialDto>>
{
	public async Task<Result<ShadowingMaterialDto>> Handle(
		GetShadowingMaterialByIdQuery request, CancellationToken cancellationToken)
	{
		var material = await repository.GetByIdAsync(ShadowingMaterialId.From(request.MaterialId), cancellationToken);
		if (material is null)
			return Result.Failure<ShadowingMaterialDto>(ShadowingMaterialErrors.NotFound);

		if (currentUser.Role != RoleCodes.LinguisticCurator && material.Status != ShadowingMaterialStatus.Ready)
			return Result.Failure<ShadowingMaterialDto>(ShadowingMaterialErrors.NotAvailable);

		return Result.Success(await ShadowingMaterialDto.FromEntityWithDetailsAsync(
			material, roleRepository, roleLineRepository, storageService,
			request.LinePageNumber, request.LinePageSize, cancellationToken));
	}
}
