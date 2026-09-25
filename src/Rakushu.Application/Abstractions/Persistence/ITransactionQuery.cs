using Rakushu.Application.Usecases.Transaction.GetTransactionsByPaymentId;

namespace Rakushu.Application.Abstractions.Persistence;

public interface ITransactionQuery
{
	Task<IReadOnlyCollection<TransactionDto>> GetTransactionsByPaymentAsync(
		GetTransactionsByPaymentIdQuery query,
		CancellationToken cancellationToken = default);
}

public sealed record TransactionDto(
	Guid Id,
	Guid PaymentId,
	string Provider,
	decimal Amount,
	string Currency,
	string TxnRef,
	string Url,
	string? TransactionNo,
	string? RawResponsePayload,
	string Status,
	DateTime ExpiredAt,
	DateTime CreatedAt,
	DateTime UpdatedAt
);
