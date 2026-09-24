using Dapper;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Application.Usecases.PlanEntitlement.GetPlanEntitlements;
using Rakushu.Domain.Entities.Plan;
using Rakushu.Persistence.Connection;

namespace Rakushu.Persistence.Queries;

internal sealed class PlanEntitlementQuery : IPlanEntitlementQuery
{
	private readonly IDbConnectionFactory _connectionFactory;

	public PlanEntitlementQuery(IDbConnectionFactory connectionFactory)
	{
		_connectionFactory = connectionFactory;
	}

	public async Task<IReadOnlyCollection<PlanEntitlementDto>> GetByPlanIdAsync(
		PlanId planId,
		CancellationToken cancellationToken = default)
	{
		await using var connection = _connectionFactory.CreateConnection();

		const string sql = """
			SELECT 
				pe.id AS Id,
				pe.plan_id AS PlanId,
				pe.feature_id AS FeatureId,
				f.code AS FeatureCode,
				f.name AS FeatureName,
				pe.is_enabled AS IsEnabled,
				pe.limit_value AS LimitValue,
				pe.limit_unit AS LimitUnit,
				pe.limit_period AS LimitPeriod
			FROM plan_entitlements pe
			JOIN features f ON pe.feature_id = f.id
			WHERE pe.plan_id = @PlanId
			ORDER BY f.name ASC;
			""";

		var items = await connection.QueryAsync<PlanEntitlementDto>(
			new CommandDefinition(
				sql,
				new { PlanId = planId.Value },
				cancellationToken: cancellationToken));

		return items.ToList();
	}
}
