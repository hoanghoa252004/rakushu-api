using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Domain.Entities.Identity.User;

public interface IUserRepository : IBaseRepository<User, UserId>
{
	Task<(IReadOnlyList<User> Items, int TotalCount)> GetPagedAsync(
		Guid? roleId,
		UserStatus? status,
		string? searchTerm,
		int pageNumber,
		int pageSize,
		CancellationToken cancellationToken = default);
	Task<User?> GetProfileByIdAsync(UserId id, CancellationToken cancellationToken = default);
	Task<User?> GetProfileByEmailAsync(string email, CancellationToken cancellationToken = default);
	Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);	Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
	Task<IEnumerable<User>> GetByRoleIdAsync(RoleId roleId, CancellationToken cancellationToken = default);
}
