using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Domain.Repositories;

public interface IRoleRepository : IBaseRepository<Role, RoleId>
{
	Task<Role?> GetByTitleAsync(string title, CancellationToken cancellationToken = default);
}
