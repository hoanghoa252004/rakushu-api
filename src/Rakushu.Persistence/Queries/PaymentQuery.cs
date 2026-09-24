using Dapper;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Application.Usecases.Payment.GetMyPayments;
using Rakushu.Application.Usecases.Payment.GetPaymentById;
using Rakushu.Application.Usecases.Payment.GetPayments;
using Rakushu.Domain.Entities.Payment;
using Rakushu.Domain.Entities.User;
using Rakushu.Persistence.Connection;

namespace Rakushu.Persistence.Queries;

internal sealed class PaymentQuery : IPaymentQuery
{
	private readonly IDbConnectionFactory _connectionFactory;

	public PaymentQuery(IDbConnectionFactory connectionFactory)
	{
		_connectionFactory = connectionFactory;
	}

	public async Task<PaymentDto?> GetByIdAsync(PaymentId id, CancellationToken cancellationToken = default)
	{
		await using var connection = _connectionFactory.CreateConnection();

		const string sql = """
			SELECT 
				p.id,
				p.user_id,
				p.plan_id,
				pl.name AS plan_name,
				p.subscription_id,
				p.order_code,
				p.amount,
				p.currency,
				p.status,
				p.description,
				p.qr_code_url,
				p.expires_at,
				p.completed_at,
				p.created_at
			FROM payments p
			LEFT JOIN plans pl ON pl.id = p.plan_id
			WHERE p.id = @Id;

			SELECT 
				pt.id,
				pt.sepay_id,
				pt.gateway,
				pt.account_number,
				pt.transaction_date,
				pt.content,
				pt.transfer_amount,
				pt.reference_code,
				pt.status,
				pt.expires_at,
				pt.created_at
			FROM payment_transactions pt
			WHERE pt.payment_id = @Id
			ORDER BY pt.created_at DESC;
			""";

		using var multi = await connection.QueryMultipleAsync(
			new CommandDefinition(sql, new { Id = id.Value }, cancellationToken: cancellationToken));

		var paymentRow = await multi.ReadSingleOrDefaultAsync<PaymentDbRow>();
		if (paymentRow == null)
		{
			return null;
		}

		var transactions = (await multi.ReadAsync<PaymentTransactionDto>()).ToList();

		return new PaymentDto(
			paymentRow.Id,
			paymentRow.UserId,
			paymentRow.PlanId,
			paymentRow.PlanName ?? string.Empty,
			paymentRow.SubscriptionId,
			paymentRow.OrderCode,
			paymentRow.Amount,
			paymentRow.Currency,
			paymentRow.Status,
			paymentRow.Description,
			paymentRow.QrCodeUrl,
			paymentRow.ExpiresAt,
			paymentRow.CompletedAt,
			paymentRow.CreatedAt,
			transactions);
	}

	public async Task<(IReadOnlyCollection<PaymentSummaryDto> Items, int TotalCount)> GetMyPaymentsAsync(
		UserId userId,
		int pageNumber,
		int pageSize,
		CancellationToken cancellationToken = default)
	{
		await using var connection = _connectionFactory.CreateConnection();

		const string sql = """
			SELECT 
				p.id,
				p.order_code,
				p.plan_id,
				pl.name AS plan_name,
				p.amount,
				p.currency,
				p.status,
				p.description,
				p.completed_at,
				p.created_at,
				p.expires_at
			FROM payments p
			LEFT JOIN plans pl ON pl.id = p.plan_id
			WHERE p.user_id = @UserId
			ORDER BY p.created_at DESC
			LIMIT @PageSize OFFSET @Offset;

			SELECT COUNT(*)
			FROM payments
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

		var items = (await multi.ReadAsync<PaymentSummaryDto>()).ToList();
		var totalCount = await multi.ReadSingleAsync<int>();

		return (items, totalCount);
	}

	public async Task<(IReadOnlyCollection<PaymentAdminDto> Items, int TotalCount)> GetPaymentsAsync(
		GetPaymentsQuery query,
		CancellationToken cancellationToken = default)
	{
		await using var connection = _connectionFactory.CreateConnection();

		const string sql = """
			SELECT 
				p.id,
				p.user_id,
				u.email AS user_email,
				p.plan_id,
				pl.name AS plan_name,
				p.subscription_id,
				p.order_code,
				p.amount,
				p.currency,
				p.status,
				p.description,
				p.completed_at,
				p.created_at,
				p.expires_at
			FROM payments p
			LEFT JOIN users u ON u.id = p.user_id
			LEFT JOIN plans pl ON pl.id = p.plan_id
			WHERE 
				(@Status IS NULL OR p.status = @Status)
				AND (@OrderCode IS NULL OR p.order_code ILIKE '%' || @OrderCode || '%')
				AND (@UserId IS NULL OR p.user_id = @UserId)
				AND (@PlanId IS NULL OR p.plan_id = @PlanId)
			ORDER BY p.created_at DESC
			LIMIT @PageSize OFFSET @Offset;

			SELECT COUNT(*)
			FROM payments p
			WHERE 
				(@Status IS NULL OR p.status = @Status)
				AND (@OrderCode IS NULL OR p.order_code ILIKE '%' || @OrderCode || '%')
				AND (@UserId IS NULL OR p.user_id = @UserId)
				AND (@PlanId IS NULL OR p.plan_id = @PlanId);
			""";

		var parameters = new
		{
			query.Status,
			query.OrderCode,
			query.UserId,
			query.PlanId,
			query.PageSize,
			Offset = (query.PageNumber - 1) * query.PageSize
		};

		using var multi = await connection.QueryMultipleAsync(
			new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

		var items = (await multi.ReadAsync<PaymentAdminDto>()).ToList();
		var totalCount = await multi.ReadSingleAsync<int>();

		return (items, totalCount);
	}

	public async Task<IReadOnlyCollection<PaymentTransactionDto>> GetTransactionsByPaymentIdAsync(
		PaymentId paymentId,
		CancellationToken cancellationToken = default)
	{
		await using var connection = _connectionFactory.CreateConnection();

		const string sql = """
			SELECT 
				pt.id,
				pt.sepay_id,
				pt.gateway,
				pt.account_number,
				pt.transaction_date,
				pt.content,
				pt.transfer_amount,
				pt.reference_code,
				pt.status,
				pt.expires_at,
				pt.created_at
			FROM payment_transactions pt
			WHERE pt.payment_id = @PaymentId
			ORDER BY pt.created_at DESC;
			""";

		var transactions = await connection.QueryAsync<PaymentTransactionDto>(
			new CommandDefinition(sql, new { PaymentId = paymentId.Value }, cancellationToken: cancellationToken));

		return transactions.ToList();
	}

	private sealed class PaymentDbRow
	{
		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public Guid PlanId { get; set; }
		public string? PlanName { get; set; }
		public Guid? SubscriptionId { get; set; }
		public string OrderCode { get; set; } = string.Empty;
		public decimal Amount { get; set; }
		public string Currency { get; set; } = string.Empty;
		public string Status { get; set; } = string.Empty;
		public string? Description { get; set; }
		public string? QrCodeUrl { get; set; }
		public DateTimeOffset ExpiresAt { get; set; }
		public DateTimeOffset? CompletedAt { get; set; }
		public DateTimeOffset CreatedAt { get; set; }
	}
}
