using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Domain.Entities.Payment.PaymentTransaction;

public sealed class PaymentTransaction : Entity<PaymentTransactionId>
{
	public PaymentId PaymentId { get; private set; } = null!;
	public long SepayId { get; private set; }
	public string Gateway { get; private set; } = null!;
	public string AccountNumber { get; private set; } = null!;
	public DateTimeOffset TransactionDate { get; private set; }
	public string Content { get; private set; } = null!;
	public string TransferType { get; private set; } = "in";
	public decimal TransferAmount { get; private set; }
	public string? ReferenceCode { get; private set; }
	public TransactionStatus Status { get; private set; }
	public string? RawWebhookData { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	public Payment Payment { get; private set; } = null!;

	private PaymentTransaction() { }

	private PaymentTransaction(
		PaymentTransactionId id,
		PaymentId paymentId,
		long sepayId,
		string gateway,
		string accountNumber,
		DateTimeOffset transactionDate,
		string content,
		string transferType,
		decimal transferAmount,
		string? referenceCode,
		TransactionStatus status,
		string? rawWebhookData,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt) : base(id)
	{
		PaymentId = paymentId;
		SepayId = sepayId;
		Gateway = gateway;
		AccountNumber = accountNumber;
		TransactionDate = transactionDate;
		Content = content;
		TransferType = transferType;
		TransferAmount = transferAmount;
		ReferenceCode = referenceCode;
		Status = status;
		RawWebhookData = rawWebhookData;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}

	public static Result<PaymentTransaction> Create(
		PaymentId paymentId,
		long sepayId,
		string gateway,
		string accountNumber,
		DateTimeOffset transactionDate,
		string content,
		string transferType,
		decimal transferAmount,
		string? referenceCode,
		TransactionStatus status,
		string? rawWebhookData,
		DateTimeOffset now)
	{
		var transaction = new PaymentTransaction(
			PaymentTransactionId.Create(),
			paymentId,
			sepayId,
			gateway,
			accountNumber,
			transactionDate,
			content,
			transferType,
			transferAmount,
			referenceCode,
			status,
			rawWebhookData,
			now,
			now);

		return Result.Success(transaction);
	}

	public void MarkSuccess(DateTimeOffset now)
	{
		Status = TransactionStatus.Success;
		UpdatedAt = now;
	}

	public void MarkFailed(DateTimeOffset now)
	{
		Status = TransactionStatus.Failed;
		UpdatedAt = now;
	}

	public void MarkExpired(DateTimeOffset now)
	{
		Status = TransactionStatus.Expired;
		UpdatedAt = now;
	}
}
