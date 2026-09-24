namespace Rakushu.Application.Usecases.Payment.GetPaymentById;

public sealed record PaymentDto(
	Guid Id,
	Guid UserId,
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

public sealed record PaymentTransactionDto
{
	public Guid Id { get; init; }
	public long? SepayId { get; init; }
	public string Gateway { get; init; } = string.Empty;
	public string AccountNumber { get; init; } = string.Empty;
	public DateTimeOffset TransactionDate { get; init; }
	public string Content { get; init; } = string.Empty;
	public decimal TransferAmount { get; init; }
	public string? ReferenceCode { get; init; }
	public string Status { get; init; } = string.Empty;
	public DateTimeOffset ExpiresAt { get; init; }
	public DateTimeOffset CreatedAt { get; init; }

	public PaymentTransactionDto() { }

	public PaymentTransactionDto(
		Guid id,
		long? sepayId,
		string gateway,
		string accountNumber,
		DateTimeOffset transactionDate,
		string content,
		decimal transferAmount,
		string? referenceCode,
		string status,
		DateTimeOffset expiresAt,
		DateTimeOffset createdAt)
	{
		Id = id;
		SepayId = sepayId;
		Gateway = gateway;
		AccountNumber = accountNumber;
		TransactionDate = transactionDate;
		Content = content;
		TransferAmount = transferAmount;
		ReferenceCode = referenceCode;
		Status = status;
		ExpiresAt = expiresAt;
		CreatedAt = createdAt;
	}
}
