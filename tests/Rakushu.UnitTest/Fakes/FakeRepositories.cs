using System.Linq.Expressions;
using MediatR;
using Rakushu.Application.Abstractions.Authentication;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Role;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.User.RefreshToken;
using Rakushu.Domain.Repositories;

namespace Rakushu.UnitTest.Fakes;

public class FakeUnitOfWork : IUnitOfWork
{
	public int SaveChangesCallCount { get; private set; }
	public List<INotification> PublishedEvents { get; } = new();

	public async Task<T> ExecuteAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default)
	{
		SaveChangesCallCount++;
		return await action();
	}

	public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		SaveChangesCallCount++;
		return Task.FromResult(1);
	}
}

public class FakeUserRepository : IUserRepository
{
	public List<User> Users { get; } = new();
	public List<RefreshToken> RefreshTokens { get; } = new();

	public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
	{
		return Task.FromResult(Users.FirstOrDefault(u => u.Id == id));
	}

	public Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return Task.FromResult<IEnumerable<User>>(Users);
	}

	public Task<IEnumerable<User>> FindAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
	{
		return Task.FromResult<IEnumerable<User>>(Users.AsQueryable().Where(predicate).ToList());
	}

	public void Add(User entity) => Users.Add(entity);
	public void Update(User entity) { }
	public void Delete(User entity) => Users.Remove(entity);
	public void DeleteMultiple(List<User> entities) => entities.ForEach(e => Users.Remove(e));

	public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
	{
		return Task.FromResult(Users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)));
	}

	public Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
	{
		return Task.FromResult(Users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)));
	}

	public Task<User?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
	{
		return Task.FromResult(Users.FirstOrDefault(u => u.Id == id));
	}

	public Task<Profile?> GetProfileByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
	{
		return Task.FromResult(Users.FirstOrDefault(u => u.Id == userId)?.Profile);
	}

	public Task<bool> IsEmailUniqueAsync(string email, Guid? excludeUserId = null, CancellationToken cancellationToken = default)
	{
		var query = Users.Where(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
		if (excludeUserId.HasValue) query = query.Where(u => u.Id != excludeUserId.Value);
		return Task.FromResult(!query.Any());
	}

	public Task<bool> IsUsernameUniqueAsync(string username, Guid? excludeUserId = null, CancellationToken cancellationToken = default)
	{
		var query = Users.Where(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
		if (excludeUserId.HasValue) query = query.Where(u => u.Id != excludeUserId.Value);
		return Task.FromResult(!query.Any());
	}

	public Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
	{
		return Task.FromResult(RefreshTokens.FirstOrDefault(t => t.Token == token));
	}

	public Task RevokeUserRefreshTokensAsync(Guid userId, CancellationToken cancellationToken = default)
	{
		foreach (var t in RefreshTokens.Where(t => t.UserId == userId))
		{
			t.Revoke();
		}
		return Task.CompletedTask;
	}

	public void AddRefreshToken(RefreshToken token)
	{
		RefreshTokens.Add(token);
	}

	public Task<(IReadOnlyList<User> Items, int TotalCount)> GetPagedAsync(
		int pageNumber,
		int pageSize,
		string? searchTerm = null,
		Guid? roleId = null,
		string? status = null,
		CancellationToken cancellationToken = default)
	{
		var query = Users.AsQueryable();
		if (!string.IsNullOrWhiteSpace(searchTerm))
		{
			query = query.Where(u => u.Username.Contains(searchTerm) || u.Email.Contains(searchTerm));
		}
		if (roleId.HasValue && roleId.Value != Guid.Empty)
		{
			query = query.Where(u => u.RoleId == roleId.Value);
		}
		if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<UserStatus>(status, true, out var parsedStatus))
		{
			query = query.Where(u => u.Status == parsedStatus);
		}
		var total = query.Count();
		var items = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
		return Task.FromResult(((IReadOnlyList<User>)items, total));
	}
}

public class FakeRoleRepository : IRoleRepository
{
	public List<Role> Roles { get; } = new();

	public Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
		Task.FromResult(Roles.FirstOrDefault(r => r.Id == id));

	public Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken = default) =>
		Task.FromResult<IEnumerable<Role>>(Roles);

	public Task<IEnumerable<Role>> FindAsync(Expression<Func<Role, bool>> predicate, CancellationToken cancellationToken = default) =>
		Task.FromResult<IEnumerable<Role>>(Roles.AsQueryable().Where(predicate).ToList());

	public void Add(Role entity) => Roles.Add(entity);
	public void Update(Role entity) { }
	public void Delete(Role entity) => Roles.Remove(entity);
	public void DeleteMultiple(List<Role> entities) => entities.ForEach(e => Roles.Remove(e));

	public Task<Role?> GetByNameAsync(string roleName, CancellationToken cancellationToken = default) =>
		Task.FromResult(Roles.FirstOrDefault(r => r.RoleName.Equals(roleName, StringComparison.OrdinalIgnoreCase)));

	public Task<IReadOnlyList<Role>> GetAllRolesAsync(CancellationToken cancellationToken = default) =>
		Task.FromResult<IReadOnlyList<Role>>(Roles);
}

public class FakePasswordHasher : IPasswordHasher
{
	public string HashPassword(string password) => $"hashed_{password}";
	public bool VerifyPassword(string password, string passwordHash) => passwordHash == $"hashed_{password}";
}

public class FakeJwtTokenGenerator : IJwtTokenGenerator
{
	public string GenerateAccessToken(Guid userId, string email, string username, string roleName) => "fake_access_token";
	public (string Token, DateTimeOffset ExpiresAt) GenerateRefreshToken() => ("fake_refresh_token", DateTimeOffset.UtcNow.AddDays(7));
	public int GetAccessTokenExpirationMinutes() => 60;
	public int GetRefreshTokenExpirationDays() => 7;
}

public class FakeCurrentUserContext : ICurrentUserContext
{
	public Guid? UserId { get; set; }
	public string? Email { get; set; }
	public string? Role { get; set; }
	public bool IsAuthenticated => UserId.HasValue;
}
