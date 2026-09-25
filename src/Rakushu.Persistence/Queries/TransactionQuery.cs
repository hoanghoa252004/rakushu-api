using Dapper;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Application.Usecases.Transaction.GetTransactionsByPaymentId;
using Rakushu.Persistence.Connection;
using System.Text;

namespace Rakushu.Persistence.Queries;

internal class TransactionQuery : ITransactionQuery
{
	private readonly IDbConnectionFactory _connection;

	public TransactionQuery(IDbConnectionFactory connection)
	{
		_connection = connection;
	}

	public async Task<IReadOnlyCollection<TransactionDto>> GetTransactionsByPaymentAsync(
		GetTransactionsByPaymentIdQuery query,
		CancellationToken cancellationToken = default)
	{
		await using var connection = _connection.CreateConnection();

		const string sql = """
			SELECT
				t.id,
				t.payment_id,
				t.provider,
				t.amount,
				t.currency,
				t.txn_ref,
				t.url,
				t.transaction_no,
				t.raw_response_payload,
				t.status,
				t.expired_at,
				t.created_at,
				t.updated_at
			FROM transactions t
			WHERE t.payment_id = @PaymentId
				AND 
				(@Status IS NULL OR t.status = @Status)
				AND 
				(@Provider IS NULL OR t.provider = @Provider)
			ORDER BY t.created_at DESC;
			""";

		var parameters = new
		{
			PaymentId = query.PaymentId,
			Status = string.IsNullOrWhiteSpace(query.Status)
				? null
				: query.Status,
			Provider = string.IsNullOrWhiteSpace(query.Provider)
				? null
				: query.Provider
		};

		var items = await connection.QueryAsync<TransactionDto>(
			new CommandDefinition(
				sql,
				parameters,
				cancellationToken: cancellationToken));

		return items.ToList();
	}
}
