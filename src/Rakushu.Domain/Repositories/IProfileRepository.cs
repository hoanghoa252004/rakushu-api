using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities;

namespace Rakushu.Domain.Repositories;

public interface IProfileRepository : IRepository<Profile, Guid>
{
	Task<Profile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
