using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Payment.Transaction.GetTransactionsByPaymentId;

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
	DateTimeOffset ExpiredAt,
	DateTimeOffset CreatedAt,
	DateTimeOffset UpdatedAt
);