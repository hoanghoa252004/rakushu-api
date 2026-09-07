using MediatR;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Admin.Roles.GetRoles;

internal sealed class GetRolesHandler : IRequestHandler<GetRolesQuery, Result<IReadOnlyCollection<GetRolesDto>>>
{
	// DAOs
	private readonly IRoleQuery _roleQuery;

	public GetRolesHandler(IRoleQuery roleQuery)
	{
		_roleQuery = roleQuery;
	}

	public async Task<Result<IReadOnlyCollection<GetRolesDto>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
	{
		var roles = await _roleQuery.GetRoles(cancellationToken);

		return Result.Success(roles);
	}
}
