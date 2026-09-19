using Dapper;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Application.Usecases.Plan.GetPlanById;
using Rakushu.Application.Usecases.Plan.GetPlans;
using Rakushu.Domain.Entities.Plan;
using Rakushu.Persistence.Connection;

namespace Rakushu.Persistence.Queries;

internal sealed class PlanQuery : IPlanQuery
{
	private readonly IDbConnectionFactory _connectionFactory;

	public PlanQuery(IDbConnectionFactory connectionFactory)
	{
		_connectionFactory = connectionFactory;
	}

	public async Task<PlanDto?> GetByIdAsync(PlanId id, CancellationToken cancellationToken = default)
	{
		await using var connection = _connectionFactory.CreateConnection();

		const string sql = """
			SELECT 
				id,
				code,
				name,
				price,
				currency,
				billing_cycle,
				status,
				created_at,
				updated_at,
				description
			FROM plans
			WHERE id = @Id
			""";

		return await connection.QuerySingleOrDefaultAsync<PlanDto>(
			new CommandDefinition(
				sql,
				new
				{
					Id = id.Value
				},
				cancellationToken: cancellationToken));

		/*
		return await _dbContext.Plans
			.AsNoTracking()
			.Where(p => p.Id == id)
			.Select(p => new PlanDto(
				p.Id.Value,
				p.Code.Value,
				p.Name,
				p.Price,
				p.Currency.ToString(),
				p.BillingCycle.ToString(),
				p.Status.ToString(),
				p.CreatedAt,
				p.UpdatedAt,
				p.Description))
			.SingleOrDefaultAsync(cancellationToken);
		*/
	}

	public async Task<(IReadOnlyCollection<PlanDto> Items, int TotalCount)> GetPlansAsync(GetPlansQuery query, CancellationToken cancellationToken = default)
	{
		await using var connection = _connectionFactory.CreateConnection();

		const string sql = """
			SELECT
				id,
				code,
				name,
				price,
				currency,
				billing_cycle,
				status,
				created_at,
				updated_at,
				description
			FROM plans p
			WHERE
				(@Status IS NULL OR p.status = @Status)
				AND
				(
					@FeatureCount = 0
					OR
					(
						SELECT COUNT(DISTINCT pe.id)
						FROM plan_entitlements pe
						WHERE 
							pe.plan_id = p.id
						AND 
							pe.feature_id = ANY(@FeatureIds)
					) = @FeatureCount
				)
			ORDER BY p.created_at DESC
			LIMIT @PageSize
			OFFSET @Offset;

			SELECT COUNT(*)
			FROM plans p
			WHERE
				(@Status IS NULL OR p.status = @Status)
				AND
				(
					@FeatureCount = 0
					OR
					(
						SELECT COUNT(DISTINCT pe.feature_id)
						FROM plan_entitlements pe
						WHERE pe.plan_id = p.id
						  AND pe.feature_id = ANY(@FeatureIds)
					) = @FeatureCount
				);
			""";

		var parameters = new
		{
			Status = query.Status?.ToString(),
			FeatureIds = query.FeatureIds?.ToArray() ?? Array.Empty<Guid>(),
			FeatureCount = query.FeatureIds?.Count ?? 0,
			query.PageSize,
			Offset = (query.PageNumber - 1) * query.PageSize
		};

		using var multi = await connection.QueryMultipleAsync(
					new CommandDefinition(
						sql,
						parameters,
						cancellationToken: cancellationToken));

		var items = (await multi.ReadAsync<PlanDto>()).ToList();

		var totalCount = await multi.ReadSingleAsync<int>();

		return (items,  totalCount);
		/*
		IQueryable<Plan> plans = _dbContext.Plans.AsNoTracking();

		// Filter by status
		if (query.Status != null)
		{
			plans = plans.Where(u => u.Status == query.Status);
		}

		// Filter by Features
		if (query.FeatureIds != null && query.FeatureIds.Any() == true)
		{
			//Collection<FeatureId> featIds = new();
			//foreach (var featureId in query.FeatureIds)
			//{
			//	featIds.Add(FeatureId.From(featureId));
			//}
			//plans = plans.Where(
			//	p => featIds.All(
			//		id => p.PlanEntitlements.Any(
			//			pe => pe.FeatureId == id)));
		}

		// Count BEFORE pagination
		var totalCount = await plans
			.CountAsync(cancellationToken);

		// Pagination + Projection
		var items = await plans
			.OrderBy(u => u.Id)
			.Skip((query.PageNumber - 1) * query.PageSize)
			.Take(query.PageSize)
			.Select(p => new PlanDto(
				p.Id.Value,
				p.Code.Value,
				p.Name,
				p.Price,
				p.Currency.ToString(),
				p.BillingCycle.ToString(),
				p.Status.ToString(),
				p.CreatedAt,
				p.UpdatedAt,
				p.Description))
			.ToListAsync(cancellationToken);

		return (items, totalCount);

		*/
	}
}
