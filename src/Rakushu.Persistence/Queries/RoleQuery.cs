using Microsoft.EntityFrameworkCore;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Application.Usecases.Admin.Roles.GetRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Persistence.Queries;

internal class RoleQuery : IRoleQuery
{
	private readonly RakushuDbContext _dbContext;

	public RoleQuery(RakushuDbContext dbContext)
	{
		_dbContext = dbContext;
	}
	public async Task<IReadOnlyCollection<GetRolesDto>> GetRoles(CancellationToken cancellationToken = default)
	{
		return await _dbContext.Roles
			.AsNoTracking()
			.Select(role => new GetRolesDto(
				role.Id.Value,
				role.Title, 
				role.Description,
				role.CreatedAt,
				role.UpdatedAt))
			.ToListAsync(cancellationToken);
	}
}
