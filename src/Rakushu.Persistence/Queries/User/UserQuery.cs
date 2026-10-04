using Dapper;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Application.Usecases.User.User.GetUserById;
using Rakushu.Application.Usecases.User.User.GetUsers;
using Rakushu.Domain.Entities.User;
using Rakushu.Persistence.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Persistence.Queries.User;

internal class UserQuery : IUserQuery
{
	private readonly IDbConnectionFactory _connection;

	public UserQuery(IDbConnectionFactory connection)
	{
		_connection = connection;
	}

	public async Task<UserDetailDto?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default)
	{
		await using var connection = _connection.CreateConnection();

		const string sql = """
			SELECT
			    u.id AS "Id",
			    u.email AS "Email",
			    u.status AS "Status",
			    u.created_at AS "CreatedAt",
			    u.updated_at AS "UpdatedAt",

				r.id AS "RoleId",
				r.code AS "RoleCode",
				r.name AS "RoleName",

			    p.id AS "ProfileId",
			    p.full_name AS "FullName",
			    p.avatar_key AS "Avatar",
			    p.daily_learning_minutes AS "DailyLearningMinutes",
			    p.session_duration_minutes AS "SessionDurationMinutes",

			    sl.id AS "NativeLanguageId",
			    sl.code AS "NativeLanguageCode",
			    sl.name AS "NativeLanguageName",
			    sl.native_name AS "NativeLanguageNativeName",

			    cur_level.id AS "CurrentLevelId",
			    cur_level.code AS "CurrentLevelCode",
			    cur_level.name AS "CurrentLevelName",

			    cur_framework.id AS "CurrentFrameworkId",
			    cur_framework.code AS "CurrentFrameworkCode",
			    cur_framework.name AS "CurrentFrameworkName",

			    tgt_level.id AS "TargetLevelId",
			    tgt_level.code AS "TargetLevelCode",
			    tgt_level.name AS "TargetLevelName",

			    tgt_framework.id AS "TargetFrameworkId",
			    tgt_framework.code AS "TargetFrameworkCode",
			    tgt_framework.name AS "TargetFrameworkName"

			FROM users u

			INNER JOIN roles r
			    ON r.id = u.role_id
			LEFT JOIN profiles p
			    ON p.user_id = u.id
			LEFT JOIN supported_languages sl
			    ON sl.id = p.native_language_id
			LEFT JOIN proficiency_levels cur_level
			    ON cur_level.id = p.current_level_id
			LEFT JOIN proficiency_frameworks cur_framework
			    ON cur_framework.id = cur_level.proficiency_framework_id
			LEFT JOIN proficiency_levels tgt_level
			    ON tgt_level.id = p.target_level_id
			LEFT JOIN proficiency_frameworks tgt_framework
			    ON tgt_framework.id = tgt_level.proficiency_framework_id

			WHERE u.id = @Id;

			SELECT
			    i.id AS "Id",
			    i.priority AS "Priority",

			    cc.id AS "ContentCategoryId",
			    cc.slug AS "ContentCategorySlug",
			    cc.code AS "ContentCategoryCode",
			    cc.name AS "ContentCategoryName"

			FROM interests i

			INNER JOIN content_categories cc
			    ON cc.id = i.content_category_id
			INNER JOIN profiles p
			    ON p.id = i.profile_id
			WHERE p.user_id = @Id
			ORDER BY i.priority ASC;
			""";

		using var multi = await connection.QueryMultipleAsync(
			new CommandDefinition(
				sql,
				new { Id = id.Value },
				cancellationToken: cancellationToken));

		var user = await multi.ReadSingleOrDefaultAsync<UserDetailRow>();

		if (user is null)
			return null;

		var interests = (await multi.ReadAsync<UserInterestRow>())
			.Select(uir => new InterestDto(
				uir.Id,
				uir.Priority,
				new InterestedContentDto(
					uir.ContentCategoryId,
					uir.ContentCategorySlug,
					uir.ContentCategoryCode,
					uir.ContentCategoryName
					)
				)
			)
			.ToList();

		return MapToDetailDto(user, interests);
	}

	public async Task<(IReadOnlyCollection<UserListItemDto> Items, int TotalCount)> GetUsersAsync(GetUsersQuery query, CancellationToken cancellationToken = default)
	{
		await using var connection = _connection.CreateConnection();

		const string sql = """
			SELECT
		        u.id AS "Id",
		        u.email AS "Email",
		        u.status AS "Status",
		        p.full_name AS "FullName",
		        p.avatar_key AS "Avatar",
		        u.created_at AS "CreatedAt",
				u.updated_at AS "UpdatedAt",

				r.id AS "RoleId",
		        r.code AS "RoleCode",
		        r.name AS "RoleName"
		    FROM users u
		    INNER JOIN roles r
		        ON r.id = u.role_id
		    LEFT JOIN profiles p
		        ON p.user_id = u.id
		    WHERE
		        (@Status IS NULL OR u.status = @Status)
		        AND (
		            @SearchTerm IS NULL
		            OR u.email ILIKE @SearchTerm
		            OR p.full_name ILIKE @SearchTerm
		        )
		        AND (@RoleId IS NULL OR u.role_id = @RoleId)
		    ORDER BY u.created_at DESC, u.id
		    LIMIT @PageSize
		    OFFSET @Offset;

		    SELECT COUNT(*)
		    FROM users u
		    LEFT JOIN profiles p
		        ON p.user_id = u.id
		    WHERE
		        (@Status IS NULL OR u.status = @Status)
		        AND (
		            @SearchTerm IS NULL
		            OR u.email ILIKE @SearchTerm
		            OR p.full_name ILIKE @SearchTerm
		        )
		        AND (@RoleId IS NULL OR u.role_id = @RoleId);
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

		var rows = (await multi.ReadAsync<UserListRow>()).AsList();

		var totalCount = await multi.ReadSingleAsync<int>();

		var items = rows
			.Select(row => new UserListItemDto(
						row.Id,
						row.Email,
						row.Status,
						row.FullName,
						row.Avatar,
						row.CreatedAt,
						row.UpdatedAt,
						new RoleDto(row.RoleId, row.RoleCode, row.RoleName)
						)
			)
			.ToList();

		return (items, totalCount);
	}

	private static UserDetailDto MapToDetailDto(
	UserDetailRow row,
	IReadOnlyCollection<InterestDto> interests)
	{
		RoleDto role = new RoleDto( row.RoleId, row.RoleCode, row.RoleName);

		ProfileDto? profile = null;

		if (row.ProfileId.HasValue)
		{
			profile = new ProfileDto(
				row.ProfileId.Value,
				row.FullName!,
				row.Avatar,
				row.DailyLearningMinutes!.Value,
				row.SessionDurationMinutes!.Value,
				new NativeLanguageDto(
					row.NativeLanguageId!.Value,
					row.NativeLanguageCode!,
					row.NativeLanguageName!,
					row.NativeLanguageNativeName!),
				new CurrentLevelDto(
					row.CurrentLevelId!.Value,
					row.CurrentLevelCode!,
					row.CurrentLevelName!,
					new FrameworkLevelDto(
						row.CurrentFrameworkId!.Value,
						row.CurrentFrameworkCode!,
						row.CurrentFrameworkName!)),
				new TargetLevelDto(
					row.TargetLevelId!.Value,
					row.TargetLevelCode!,
					row.TargetLevelName!,
					new FrameworkLevelDto(
						row.TargetFrameworkId!.Value,
						row.TargetFrameworkCode!,
						row.TargetFrameworkName!)),
				interests);
		}

		return new UserDetailDto(
			row.Id,
			row.Email,
			row.Status,
			row.CreatedAt,
			row.UpdatedAt,
			role,
			profile);
	}
}

