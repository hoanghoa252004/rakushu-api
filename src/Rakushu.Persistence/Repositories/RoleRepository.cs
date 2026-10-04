using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Role;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Persistence.Repositories;

public sealed class RoleRepository : BaseRepository<Role, RoleId>, IRoleRepository
{
	public RoleRepository(RakushuDbContext context) : base(context)
	{
	}

	public async Task<Role?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
	{
		return await _context.Roles
			.Include(r => r.Users)
			.FirstOrDefaultAsync(r => r.Code.ToLower() == code.ToLower(), cancellationToken);
	}

	public override async Task<Role?> GetByIdAsync(RoleId id, CancellationToken cancellationToken = default)
	{
		return await _context.Roles
			.Include(r => r.Users)
			.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
	}
}
