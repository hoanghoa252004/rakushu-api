using Dapper;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Application.Usecases.Admin.Users.GetUserById;
using Rakushu.Application.Usecases.Admin.Users.GetUsers;
using Rakushu.Domain.Entities.User;
using Rakushu.Persistence.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Persistence.Queries;

internal class UserQuery : IUserQuery
{
	private readonly IDbConnectionFactory _connection;

	public UserQuery(IDbConnectionFactory connection)
	{
		_connection = connection;
	}

	public async Task<UserDto?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default)
	{
		await using var connection = _connection.CreateConnection();

		const string sql = """
			SELECT 
				u.id,
				u.email,
				u.profile_full_name AS FullName,
				r.title AS Role,
				u.status,
				u.created_at,
				u.updated_at,
				u.profile_avatar_key AS AvatarUrl,
				u.profile_native_language AS NativeLanguage
			FROM users u
			INNER JOIN roles r ON r.id = u.role_id
			WHERE u.id = @Id
			""";

		return await connection.QuerySingleOrDefaultAsync<UserDto>(
			new CommandDefinition(
				sql,
				new
				{
					Id = id.Value
				},
				cancellationToken: cancellationToken));
	}

	public async Task<(IReadOnlyCollection<UserDto> Items, int TotalCount)> GetUsersAsync(GetUsersQuery query, CancellationToken cancellationToken = default)
	{
		await using var connection = _connection.CreateConnection();

		const string sql = """
		SELECT
			u.id,
			u.email,
			u.profile_full_name AS FullName,
			r.title AS Role,
			u.status,
			u.created_at,
			u.updated_at,
			u.profile_avatar_key AS AvatarUrl,
			u.profile_native_language AS NativeLanguage
		FROM users u
		INNER JOIN roles r ON r.id = u.role_id
		WHERE 
			(@Status IS NULL OR u.status = @Status)
			AND
			(@SearchTerm IS NULL OR u.email ILIKE @SearchTerm OR u.profile_full_name ILIKE @SearchTerm)
			AND
			(@RoleId IS NULL OR u.role_id = @RoleId)
		ORDER BY u.created_at DESC
		LIMIT @PageSize
		OFFSET @Offset;

		SELECT COUNT(*)
		FROM users u
		WHERE 
			(@Status IS NULL OR u.status = @Status)
			AND
			(@SearchTerm IS NULL OR u.email ILIKE @SearchTerm OR u.profile_full_name ILIKE @SearchTerm)
			AND
			(@RoleId IS NULL OR u.role_id = @RoleId);
		""";

		var parameters = new
		{
			Status = query.Status,
			SearchTerm = string.IsNullOrWhiteSpace(query.SearchTerm)
							? null
							: $"%{query.SearchTerm.Trim()}%",
			RoleId = query.RoleId,
			PageSize = query.PageSize,
			Offset = (query.PageNumber - 1) * query.PageSize
		};

		using var multi = await connection.QueryMultipleAsync(
			new CommandDefinition(
				sql,
				parameters,
				cancellationToken: cancellationToken));

		var items = (await multi.ReadAsync<UserDto>()).ToList();

		var totalCount = await multi.ReadSingleAsync<int>();

		return (items, totalCount);
	}
}

