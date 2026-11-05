using FluentValidation;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;
using Rakushu.Domain.Entities.Shadowing.ShadowingRole;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingRoles.GetShadowingRoles;

public sealed record GetShadowingRolesQuery(Guid MaterialId) : IRequest<Result<IReadOnlyList<ShadowingRoleDto>>>;

internal sealed class GetShadowingRolesValidator : AbstractValidator<GetShadowingRolesQuery>
{
	public GetShadowingRolesValidator()
	{
		RuleFor(x => x.MaterialId).NotEmpty();
	}
}

internal sealed class GetShadowingRolesHandler(IShadowingRoleRepository repository, IStorageService storageService)
	: IRequestHandler<GetShadowingRolesQuery, Result<IReadOnlyList<ShadowingRoleDto>>>
{
	public async Task<Result<IReadOnlyList<ShadowingRoleDto>>> Handle(
		GetShadowingRolesQuery request, CancellationToken cancellationToken)
	{
		var roles = await repository.GetByMaterialIdAsync(ShadowingMaterialId.From(request.MaterialId), cancellationToken);
		var dtos = await Task.WhenAll(roles.Select(r => ShadowingRoleDto.FromEntityAsync(r, storageService, cancellationToken)));
		return Result.Success<IReadOnlyList<ShadowingRoleDto>>(dtos);
	}
}
