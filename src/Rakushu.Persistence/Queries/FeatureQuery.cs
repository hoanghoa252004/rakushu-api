using Dapper;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Application.Usecases.Feature.GetFeatureById;
using Rakushu.Application.Usecases.Feature.GetFeatures;
using Rakushu.Domain.Entities.Feature;
using Rakushu.Persistence.Connection;

namespace Rakushu.Persistence.Queries;

internal sealed class FeatureQuery : IFeatureQuery
{
	private readonly IDbConnectionFactory _connection;

	public FeatureQuery(IDbConnectionFactory connection)
	{
		_connection = connection;
	}

	public async Task<FeatureDto?> GetByIdAsync(FeatureId id, CancellationToken cancellationToken = default)
	{
		await using var connection = _connection.CreateConnection();

		const string sql = """
			SELECT 
				f.id,
				f.code,
				f.name,
				f.status,
				f.created_at,
				f.updated_at,
				f.description
			FROM features f
			WHERE f.id = @Id
			""";

		return await connection.QuerySingleOrDefaultAsync<FeatureDto>(
			new CommandDefinition(
				sql,
				new
				{
					Id = id.Value
				},
				cancellationToken: cancellationToken));
	}

	public async Task<(IReadOnlyCollection<FeatureDto> Items, int TotalCount)> GetFeaturesAsync(
		GetFeaturesQuery query,
		CancellationToken cancellationToken = default)
	{
		await using var connection = _connection.CreateConnection();

		const string sql = """
		SELECT 
			f.id,
			f.code,
			f.name,
			f.status,
			f.created_at,
			f.updated_at,
			f.description
		FROM features f
		WHERE 
			(@Status IS NULL OR f.status = @Status)
			AND
			(@SearchTerm IS NULL OR f.name ILIKE @SearchTerm OR f.description ILIKE @SearchTerm)
		ORDER BY f.created_at DESC
		LIMIT @PageSize
		OFFSET @Offset;

		SELECT COUNT(*)
		FROM features f
		WHERE 
			(@Status IS NULL OR f.status = @Status)
			AND
			(@SearchTerm IS NULL OR f.name ILIKE @SearchTerm OR f.description ILIKE @SearchTerm)
		""";

		var parameters = new
		{
			Status = query.Status,
			SearchTerm = string.IsNullOrWhiteSpace(query.SearchTerm)
							? null
							: $"%{query.SearchTerm.Trim()}%",
			PageSize = query.PageSize,
			Offset = (query.PageNumber - 1) * query.PageSize
		};

		using var multi = await connection.QueryMultipleAsync(
			new CommandDefinition(
				sql,
				parameters,
				cancellationToken: cancellationToken));

		var items = (await multi.ReadAsync<FeatureDto>()).ToList();

		var totalCount = await multi.ReadSingleAsync<int>();

		return (items, totalCount);
	}
}
