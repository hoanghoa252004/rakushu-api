using Dapper;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Application.Usecases.Payment.GetMyPaymentHistory;
using Rakushu.Application.Usecases.Payment.GetPayments;
using Rakushu.Application.Usecases.Users.GetUserById;
using Rakushu.Domain.Entities.Payment;
using Rakushu.Domain.Entities.User;
using Rakushu.Persistence.Connection;
using System.Text;

namespace Rakushu.Persistence.Queries;

internal class PaymentQuery : IPaymentQuery
{
	private readonly IDbConnectionFactory _connection;

	public PaymentQuery(IDbConnectionFactory connection)
	{
		_connection = connection;
	}

	public async Task<(IReadOnlyCollection<PaymentDto> Items, int TotalCount)> GetPaymentsAsync(
		GetPaymentsQuery query,
		CancellationToken cancellationToken = default)
	{
		await using var connection = _connection.CreateConnection();

		const string sql = """
		SELECT
			p.id,
			p.user_id,
			p.plan_id,
			p.amount,
			p.currency,
			p.status,
			p.expired_at,
			p.created_at,
			p.updated_at
		FROM payments p
		INNER JOIN users u ON u.id = p.user_id
		INNER JOIN plans pl ON pl.id = p.plan_id
		WHERE
			(@UserId IS NULL OR p.user_id = @UserId)
			AND
			(@PlanId IS NULL OR p.plan_id = @PlanId)
			AND
			(@Status IS NULL OR p.status = @Status)
		ORDER BY p.created_at DESC
		LIMIT @PageSize
		OFFSET @Offset;

		SELECT COUNT(*)
		FROM payments p
		WHERE
			(@UserId IS NULL OR p.user_id = @UserId)
			AND
			(@PlanId IS NULL OR p.plan_id = @PlanId)
			AND
			(@Status IS NULL OR p.status = @Status);
		""";

		var parameters = new
		{
			UserId = query.UserId,
			PlanId = query.PlanId,
			Status = string.IsNullOrWhiteSpace(query.Status)
				? null
				: query.Status,
			query.PageSize,
			Offset = (query.PageNumber - 1) * query.PageSize
		};

		using var multi = await connection.QueryMultipleAsync(
			new CommandDefinition(
				sql,
				parameters,
				cancellationToken: cancellationToken));

		var items = (await multi.ReadAsync<PaymentDto>()).ToList();

		var totalCount = await multi.ReadSingleAsync<int>();

		return (items, totalCount);
	}

	public async Task<(IReadOnlyCollection<PaymentDto> Items, int TotalCount)> GetMyPaymentsAsync(
		UserId userId,
		GetMyPaymentHistoryQuery query,
		CancellationToken cancellationToken = default)
	{
		await using var connection = _connection.CreateConnection();

		const string sql = """
			SELECT
				p.id,
				p.user_id,
				p.plan_id,
				p.amount,
				p.currency,
				p.status,
				p.expired_at,
				p.created_at,
				p.updated_at
			FROM payments p
			INNER JOIN plans pl ON pl.id = p.plan_id
			WHERE 
				p.user_id = @UserId
				AND
				(@PlanId IS NULL OR p.plan_id = @PlanId)
				AND
				(@Status IS NULL OR p.status = @Status)
			ORDER BY p.created_at DESC
			LIMIT @PageSize
			OFFSET @Offset;

			SELECT COUNT(*)
			FROM payments p
			WHERE 
				p.user_id = @UserId
				AND
				(@PlanId IS NULL OR p.plan_id = @PlanId)
				AND
				(@Status IS NULL OR p.status = @Status)
			""";
		var parameters = new
		{
			UserId = userId.Value,
			PlanId = query.PlanId,
			Status = string.IsNullOrWhiteSpace(query.Status)
						? null
						: query.Status,
			Offset = (query.PageNumber - 1) * query.PageSize,
			PageSize = query.PageSize
		};

		using var multi = await connection.QueryMultipleAsync(
			new CommandDefinition(
				sql,
				parameters,
				cancellationToken: cancellationToken));

		var items = (await multi.ReadAsync<PaymentDto>()).ToList();

		var totalCount = await multi.ReadSingleAsync<int>();

		return (items, totalCount);
	}

	public async Task<PaymentDto?> GetPaymentByIdAsync(PaymentId paymentId, CancellationToken cancellationToken = default)
	{
		await using var connection = _connection.CreateConnection();

		const string sql = """
			SELECT
				p.id,
				p.user_id,
				p.plan_id,
				p.amount,
				p.currency,
				p.status,
				p.expired_at,
				p.created_at,
				p.updated_at
			FROM payments p
			WHERE @Id = p.id;
			""";

		return await connection.QuerySingleOrDefaultAsync<PaymentDto>(
			new CommandDefinition(
				sql,
				new
				{
					Id = paymentId.Value
				},
				cancellationToken: cancellationToken));
	}
}
