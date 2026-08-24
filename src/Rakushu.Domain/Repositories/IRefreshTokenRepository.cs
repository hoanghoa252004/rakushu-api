using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities;

namespace Rakushu.Domain.Repositories;

public interface IRefreshTokenRepository : IRepository<RefreshToken, Guid>
{
	Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
	Task RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken = default);
}
