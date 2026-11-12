using FluentValidation;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;
using Rakushu.Domain.Entities.Shadowing.ShadowingRole;
using Rakushu.Domain.Entities.Shadowing.ShadowingRoleLine;
using Rakushu.Domain.Entities.Shadowing.Speaker;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingRoleLines.GetShadowingRoleLines;

public sealed record GetShadowingRoleLinesQuery(
	Guid RoleId,
	int PageNumber,
	int PageSize) : IRequest<Result<PaginatedList<ShadowingRoleLineDto>>>;

internal sealed class GetShadowingRoleLinesValidator : AbstractValidator<GetShadowingRoleLinesQuery>
{
	public GetShadowingRoleLinesValidator()
	{
		RuleFor(x => x.RoleId).NotEmpty();
		RuleFor(x => x.PageNumber).GreaterThan(0).WithMessage("PageNumber must be greater than 0");
		RuleFor(x => x.PageSize).InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100");
	}
}

internal sealed class GetShadowingRoleLinesHandler(
	IShadowingRoleLineRepository repository,
	IShadowingRoleRepository roleRepository,
	IStorageService storageService)
	: IRequestHandler<GetShadowingRoleLinesQuery, Result<PaginatedList<ShadowingRoleLineDto>>>
{
	public async Task<Result<PaginatedList<ShadowingRoleLineDto>>> Handle(
		GetShadowingRoleLinesQuery request, CancellationToken cancellationToken)
	{
		var roleId = ShadowingRoleId.From(request.RoleId);
		var role = await roleRepository.GetByIdAsync(roleId, cancellationToken);
		if (role is null)
			return Result.Failure<PaginatedList<ShadowingRoleLineDto>>(ShadowingRoleErrors.NotFound);

		var (items, total) = await repository.GetPagedAsync(
			role.ShadowingMaterialId,
			null,
			roleId,
			request.PageNumber,
			request.PageSize,
			cancellationToken);

		var dtos = await Task.WhenAll(items.Select(l =>
			ShadowingRoleLineDto.FromEntityAsync(l, storageService, cancellationToken)));

		return Result.Success(PaginatedList<ShadowingRoleLineDto>.Create(
			dtos, total, request.PageNumber, request.PageSize));
	}
}
