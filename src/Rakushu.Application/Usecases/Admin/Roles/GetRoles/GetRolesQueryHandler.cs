using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Repositories;

namespace Rakushu.Application.Usecases.Admin.Roles.GetRoles;

internal sealed class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, Result<IReadOnlyCollection<RoleDto>>>
{
	private readonly IRoleRepository _roleRepository;

	public GetRolesQueryHandler(IRoleRepository roleRepository)
	{
		_roleRepository = roleRepository;
	}

	public async Task<Result<IReadOnlyCollection<RoleDto>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
	{
		var roles = await _roleRepository.GetAllAsync(cancellationToken);
		var dtos = roles.Select(r => new RoleDto(
			RoleId: r.Id,
			RoleName: r.RoleName,
			Description: r.Description,
			CreatedAt: r.CreatedAt
		)).ToList();

		return Result.Success<IReadOnlyCollection<RoleDto>>(dtos);
	}
}
