using Rakushu.Domain.Common.Contract;

namespace Rakushu.Domain.Entities.Identity.Role;

public interface IRoleRepository : IBaseRepository<Role, RoleId>
{
	Task<Role?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}
