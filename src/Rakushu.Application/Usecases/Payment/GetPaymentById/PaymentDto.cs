namespace Rakushu.Application.Usecases.Payment.GetPaymentById;

public sealed record PaymentDto(
	Guid Id,
	Guid PlanId,
	string PlanName,
	Guid? SubscriptionId,
	string OrderCode,
	decimal Amount,
	string Currency,
	string Status,
	string? Description,
	string? QrCodeUrl,
	DateTimeOffset ExpiresAt,
	DateTimeOffset? CompletedAt,
	DateTimeOffset CreatedAt,
	List<PaymentTransactionDto> Transactions
);

public sealed record PaymentTransactionDto(
	Guid Id,
	long SepayId,
	string Gateway,
	string AccountNumber,
	DateTimeOffset TransactionDate,
	string Content,
	decimal TransferAmount,
	string? ReferenceCode,
	string Status,
	DateTimeOffset CreatedAt
);
