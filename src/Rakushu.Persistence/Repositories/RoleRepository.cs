using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Role;
using Rakushu.Domain.Repositories;
using Rakushu.Persistence.DbContext;

namespace Rakushu.Persistence.Repositories;

public sealed class RoleRepository : Repository<Role, Guid>, IRoleRepository
{
	public RoleRepository(RakushuDbContext context) : base(context)
	{
	}

	public async Task<Role?> GetByNameAsync(string roleName, CancellationToken cancellationToken = default)
	{
		return await _context.Roles
			.FirstOrDefaultAsync(r => r.RoleName.ToLower() == roleName.ToLower(), cancellationToken);
	}

	public async Task<IReadOnlyList<Role>> GetAllRolesAsync(CancellationToken cancellationToken = default)
	{
		return await _context.Roles
			.OrderBy(r => r.RoleName)
			.ToListAsync(cancellationToken);
	}
}
