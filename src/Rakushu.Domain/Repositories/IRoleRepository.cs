using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities;

namespace Rakushu.Domain.Repositories;

public interface IRoleRepository : IRepository<Role, Guid>
{
	Task<Role?> GetByNameAsync(string roleName, CancellationToken cancellationToken = default);
}
