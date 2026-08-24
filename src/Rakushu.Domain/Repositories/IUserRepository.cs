using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities;

namespace Rakushu.Domain.Repositories;

public interface IUserRepository : IRepository<User, Guid>
{
	Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
	Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
	Task<User?> GetByIdWithProfileAndRoleAsync(Guid id, CancellationToken cancellationToken = default);
	Task<bool> IsEmailUniqueAsync(string email, Guid? excludeUserId = null, CancellationToken cancellationToken = default);
	Task<bool> IsUsernameUniqueAsync(string username, Guid? excludeUserId = null, CancellationToken cancellationToken = default);
	Task<(IEnumerable<User> Items, int TotalCount)> GetPagedAsync(
		int pageNumber,
		int pageSize,
		string? searchTerm = null,
		Guid? roleId = null,
		string? status = null,
		CancellationToken cancellationToken = default);
}
