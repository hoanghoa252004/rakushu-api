using FluentValidation;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Domain.Entities.Shadowing.ShadowingRole;
using Rakushu.Domain.Entities.Shadowing.ShadowingRoleLine;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterialRequest;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingMaterials.GetShadowingMaterials;

public sealed record GetShadowingMaterialsQuery(
	int PageNumber,
	int PageSize,
	Guid? VideoId,
	Guid? RequestId,
	MaterialType? Type,
	ShadowingMaterialStatus? Status,
	int LinePageNumber = 1,
	int LinePageSize = 20)
	: IRequest<Result<PaginatedList<ShadowingMaterialDto>>>;

internal sealed class GetShadowingMaterialsValidator : AbstractValidator<GetShadowingMaterialsQuery>
{
	public GetShadowingMaterialsValidator()
	{
		RuleFor(x => x.PageNumber).GreaterThan(0).WithMessage("PageNumber must be greater than 0");
		RuleFor(x => x.PageSize).InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100");
		RuleFor(x => x.LinePageNumber).GreaterThan(0);
		RuleFor(x => x.LinePageSize).InclusiveBetween(1, 100);
	}
}

internal sealed class GetShadowingMaterialsHandler(
	IShadowingMaterialRepository repository,
	IShadowingRoleRepository roleRepository,
	IShadowingRoleLineRepository roleLineRepository,
	IStorageService storageService,
	ICurrentUserContext currentUser)
	: IRequestHandler<GetShadowingMaterialsQuery, Result<PaginatedList<ShadowingMaterialDto>>>
{
	public async Task<Result<PaginatedList<ShadowingMaterialDto>>> Handle(
		GetShadowingMaterialsQuery request, CancellationToken cancellationToken)
	{
		// Non-curators only see Ready materials.
		var status = currentUser.Role == RoleCodes.LinguisticCurator
			? request.Status
			: ShadowingMaterialStatus.Ready;

		var (items, total) = await repository.GetPagedAsync(
			request.PageNumber,
			request.PageSize,
			request.VideoId.HasValue ? VideoId.From(request.VideoId.Value) : null,
			request.RequestId.HasValue ? ShadowingMaterialRequestId.From(request.RequestId.Value) : null,
			request.Type,
			status,
			cancellationToken);

		var dtos = new List<ShadowingMaterialDto>();
		foreach (var item in items)
			dtos.Add(await ShadowingMaterialDto.FromEntityWithDetailsAsync(
				item, roleRepository, roleLineRepository, storageService,
				request.LinePageNumber, request.LinePageSize, cancellationToken));

		return Result.Success(PaginatedList<ShadowingMaterialDto>.Create(
			dtos,
			total,
			request.PageNumber,
			request.PageSize));
	}
}
