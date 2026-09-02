using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.User.RefreshToken;

namespace Rakushu.Domain.Repositories;

public interface IUserRepository : IBaseRepository<User, UserId>
{
	Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
	Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
	//Task<(IReadOnlyList<User> Items, int TotalCount)> GetPagedAsync(
	//	int pageNumber,
	//	int pageSize,
	//	string? searchTerm = null,
	//	Guid? roleId = null,
	//	string? status = null,
	//	CancellationToken cancellationToken = default);

	//Task<User?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
	////Task<Profile?> GetProfileByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
	//Task<bool> IsEmailUniqueAsync(string email, Guid? excludeUserId = null, CancellationToken cancellationToken = default);
	//Task<bool> IsUsernameUniqueAsync(string username, Guid? excludeUserId = null, CancellationToken cancellationToken = default);
	//Task AddRefreshToken(RefreshToken refreshToken, CancellationToken cancellationToken = default);
	//Task RevokeUserRefreshTokensAsync(Guid userId, CancellationToken cancellationToken = default);
}
