using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Role;
using Rakushu.Domain.Repositories;

namespace Rakushu.Persistence.Repositories;

public sealed class RoleRepository : BaseRepository<Role, RoleId>, IRoleRepository
{
	public RoleRepository(RakushuDbContext context) : base(context)
	{
	}
	public async Task<Role?> GetByTitleAsync(string title, CancellationToken cancellationToken = default)
	{
		return await _context.Roles
			.FirstOrDefaultAsync(r => r.Title.ToLower() == title.ToLower(), cancellationToken);
	}
}
