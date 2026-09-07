using Rakushu.Domain.Common.Contract;

namespace Rakushu.Domain.Entities.Role;

public interface IRoleRepository : IBaseRepository<Role, RoleId>
{
	Task<Role?> GetByTitleAsync(string title, CancellationToken cancellationToken = default);
}
