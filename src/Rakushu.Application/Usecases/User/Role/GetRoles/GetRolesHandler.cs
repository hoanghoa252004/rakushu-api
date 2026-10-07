using MediatR;
using Rakushu.Application.Abstractions.Persistence.Queries;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Application.Usecases.User.Role.GetRoles;

internal sealed class GetRolesHandler : IRequestHandler<GetRolesQuery, Result<IReadOnlyCollection<GetRolesDto>>>
{
	// DAOs
	private readonly IRoleRepository _roleRepository;

	public GetRolesHandler(IRoleRepository roleRepository)
	{
		_roleRepository = roleRepository;
	}

	public async Task<Result<IReadOnlyCollection<GetRolesDto>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
	{
		var roles = (await _roleRepository.GetAllAsync(cancellationToken))
			.Select(role => new GetRolesDto(
				role.Id.Value,
				role.Code,
				role.Name,
				role.Description,
				role.IsActive,
				role.CreatedAt,
				role.UpdatedAt)).ToList();

		return Result.Success<IReadOnlyCollection<GetRolesDto>>(roles);
	}
}
