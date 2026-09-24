using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Domain.Entities.Payment.PaymentTransaction;

public sealed class PaymentTransaction : Entity<PaymentTransactionId>
{
	public PaymentId PaymentId { get; private set; } = null!;
	public long? SepayId { get; private set; }
	public string Gateway { get; private set; } = null!;
	public string AccountNumber { get; private set; } = null!;
	public DateTimeOffset TransactionDate { get; private set; }
	public string Content { get; private set; } = null!;
	public string TransferType { get; private set; } = "in";
	public decimal TransferAmount { get; private set; }
	public string? ReferenceCode { get; private set; }
	public TransactionStatus Status { get; private set; }
	public string? RawWebhookData { get; private set; }
	public DateTimeOffset ExpiresAt { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	public Payment Payment { get; private set; } = null!;

	private PaymentTransaction() { }

	private PaymentTransaction(
		PaymentTransactionId id,
		PaymentId paymentId,
		long? sepayId,
		string gateway,
		string accountNumber,
		DateTimeOffset transactionDate,
		string content,
		string transferType,
		decimal transferAmount,
		string? referenceCode,
		TransactionStatus status,
		string? rawWebhookData,
		DateTimeOffset expiresAt,
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
		ExpiresAt = expiresAt;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}

	public static Result<PaymentTransaction> Create(
		PaymentId paymentId,
		long? sepayId,
		string gateway,
		string accountNumber,
		DateTimeOffset transactionDate,
		string content,
		string transferType,
		decimal transferAmount,
		string? referenceCode,
		TransactionStatus status,
		string? rawWebhookData,
		DateTimeOffset expiresAt,
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
			expiresAt,
			now,
			now);

		return Result.Success(transaction);
	}

	public static Result<PaymentTransaction> CreatePending(
		PaymentId paymentId,
		string gateway,
		string accountNumber,
		string content,
		decimal transferAmount,
		DateTimeOffset expiresAt,
		DateTimeOffset now)
	{
		var transaction = new PaymentTransaction(
			PaymentTransactionId.Create(),
			paymentId,
			sepayId: null,
			gateway,
			accountNumber,
			transactionDate: now,
			content,
			transferType: "in",
			transferAmount,
			referenceCode: null,
			TransactionStatus.Pending,
			rawWebhookData: null,
			expiresAt,
			now,
			now);

		return Result.Success(transaction);
	}

	public void MarkSuccess(
		long sepayId,
		string gateway,
		string accountNumber,
		DateTimeOffset transactionDate,
		string content,
		decimal transferAmount,
		string? referenceCode,
		string? rawWebhookData,
		DateTimeOffset now)
	{
		SepayId = sepayId;
		Gateway = gateway;
		AccountNumber = accountNumber;
		TransactionDate = transactionDate;
		Content = content;
		TransferAmount = transferAmount;
		ReferenceCode = referenceCode;
		RawWebhookData = rawWebhookData;
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

	public Result Cancel(DateTimeOffset now)
	{
		if (Status != TransactionStatus.Pending)
		{
			return Result.Failure(PaymentErrors.TransactionCannotBeCanceled);
		}

		Status = TransactionStatus.Canceled;
		UpdatedAt = now;
		return Result.Success();
	}
}
