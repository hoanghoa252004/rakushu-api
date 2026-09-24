using Dapper;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Application.Usecases.Subscription.GetMySubscriptions;
using Rakushu.Application.Usecases.Subscription.GetSubscriptionById;
using Rakushu.Application.Usecases.Subscription.GetSubscriptions;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.User.Subscription;
using Rakushu.Persistence.Connection;

namespace Rakushu.Persistence.Queries;

internal sealed class SubscriptionQuery : ISubscriptionQuery
{
	private readonly IDbConnectionFactory _connectionFactory;

	public SubscriptionQuery(IDbConnectionFactory connectionFactory)
	{
		_connectionFactory = connectionFactory;
	}

	public async Task<(IReadOnlyCollection<SubscriptionSummaryDto> Items, int TotalCount)> GetMySubscriptionsAsync(
		UserId userId,
		int pageNumber,
		int pageSize,
		CancellationToken cancellationToken = default)
	{
		await using var connection = _connectionFactory.CreateConnection();

		const string sql = """
			SELECT 
				s.id,
				s.plan_id,
				pl.name AS plan_name,
				s.status,
				s.start_date,
				s.end_date,
				s.canceled_at,
				s.created_at
			FROM subscriptions s
			LEFT JOIN plans pl ON pl.id = s.plan_id
			WHERE s.user_id = @UserId
			ORDER BY s.created_at DESC
			LIMIT @PageSize OFFSET @Offset;

			SELECT COUNT(*)
			FROM subscriptions
			WHERE user_id = @UserId;
			""";

		var parameters = new
		{
			UserId = userId.Value,
			PageSize = pageSize,
			Offset = (pageNumber - 1) * pageSize
		};

		using var multi = await connection.QueryMultipleAsync(
			new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

		var items = (await multi.ReadAsync<SubscriptionSummaryDto>()).ToList();
		var totalCount = await multi.ReadSingleAsync<int>();

		return (items, totalCount);
	}

	public async Task<(IReadOnlyCollection<SubscriptionAdminDto> Items, int TotalCount)> GetSubscriptionsAsync(
		GetSubscriptionsQuery query,
		CancellationToken cancellationToken = default)
	{
		await using var connection = _connectionFactory.CreateConnection();

		const string sql = """
			SELECT 
				s.id,
				s.user_id,
				u.email AS user_email,
				s.plan_id,
				pl.name AS plan_name,
				s.status,
				s.start_date,
				s.end_date,
				s.canceled_at,
				s.created_at
			FROM subscriptions s
			LEFT JOIN users u ON u.id = s.user_id
			LEFT JOIN plans pl ON pl.id = s.plan_id
			WHERE 
				(@Status IS NULL OR s.status = @Status)
				AND (@PlanId IS NULL OR s.plan_id = @PlanId)
				AND (@UserId IS NULL OR s.user_id = @UserId)
			ORDER BY s.created_at DESC
			LIMIT @PageSize OFFSET @Offset;

			SELECT COUNT(*)
			FROM subscriptions s
			WHERE 
				(@Status IS NULL OR s.status = @Status)
				AND (@PlanId IS NULL OR s.plan_id = @PlanId)
				AND (@UserId IS NULL OR s.user_id = @UserId);
			""";

		var parameters = new
		{
			query.Status,
			query.PlanId,
			query.UserId,
			query.PageSize,
			Offset = (query.PageNumber - 1) * query.PageSize
		};

		using var multi = await connection.QueryMultipleAsync(
			new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

		var items = (await multi.ReadAsync<SubscriptionAdminDto>()).ToList();
		var totalCount = await multi.ReadSingleAsync<int>();

		return (items, totalCount);
	}

	public async Task<SubscriptionDetailDto?> GetByIdAsync(
		SubscriptionId id,
		CancellationToken cancellationToken = default)
	{
		await using var connection = _connectionFactory.CreateConnection();

		const string sql = """
			SELECT 
				s.id,
				s.user_id,
				u.email AS user_email,
				s.plan_id,
				pl.name AS plan_name,
				s.status,
				s.start_date,
				s.end_date,
				s.current_period_start,
				s.current_period_end,
				s.canceled_at,
				s.created_at
			FROM subscriptions s
			LEFT JOIN users u ON u.id = s.user_id
			LEFT JOIN plans pl ON pl.id = s.plan_id
			WHERE s.id = @Id;

			SELECT 
				f.id AS feature_id,
				f.name AS feature_name,
				f.code AS feature_code,
				pe.limit_unit AS feature_type,
				pe.limit_value,
				COALESCE(su.used_value, 0) AS used_value,
				COALESCE(su.period_start, s.current_period_start) AS period_start,
				COALESCE(su.period_end, s.current_period_end) AS period_end
			FROM subscriptions s
			INNER JOIN plan_entitlements pe ON pe.plan_id = s.plan_id AND pe.is_enabled = true
			INNER JOIN features f ON f.id = pe.feature_id
			LEFT JOIN subscription_usages su ON su.subscription_id = s.id AND su.feature_id = pe.feature_id
			WHERE s.id = @Id;

			SELECT 
				p.id,
				p.order_code,
				p.amount,
				p.currency,
				p.status,
				p.completed_at,
				p.created_at
			FROM payments p
			WHERE p.subscription_id = @Id
			ORDER BY p.created_at DESC;
			""";

		using var multi = await connection.QueryMultipleAsync(
			new CommandDefinition(sql, new { Id = id.Value }, cancellationToken: cancellationToken));

		var sub = await multi.ReadSingleOrDefaultAsync<SubscriptionDetailHeaderRow>();
		if (sub == null)
		{
			return null;
		}

		var usages = (await multi.ReadAsync<SubscriptionUsageDto>()).ToList();
		var payments = (await multi.ReadAsync<SubscriptionPaymentDto>()).ToList();

		return new SubscriptionDetailDto(
			sub.Id,
			sub.UserId,
			sub.UserEmail,
			sub.PlanId,
			sub.PlanName,
			sub.Status,
			sub.StartDate,
			sub.EndDate,
			sub.CurrentPeriodStart,
			sub.CurrentPeriodEnd,
			sub.CanceledAt,
			sub.CreatedAt,
			usages,
			payments);
	}

	private sealed class SubscriptionDetailHeaderRow
	{
		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public string? UserEmail { get; set; }
		public Guid PlanId { get; set; }
		public string PlanName { get; set; } = string.Empty;
		public string Status { get; set; } = string.Empty;
		public DateTimeOffset StartDate { get; set; }
		public DateTimeOffset EndDate { get; set; }
		public DateTimeOffset CurrentPeriodStart { get; set; }
		public DateTimeOffset CurrentPeriodEnd { get; set; }
		public DateTimeOffset? CanceledAt { get; set; }
		public DateTimeOffset CreatedAt { get; set; }
	}
}
