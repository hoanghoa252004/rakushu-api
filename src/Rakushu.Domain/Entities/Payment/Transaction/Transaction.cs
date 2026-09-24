using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Plan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Payment.Transaction;

public sealed class Transaction : Entity<TransactionId>
{
	public PaymentId PaymentId { get; private set; } = null!;
	public Provider Provider { get; private set; }
	public decimal Amount { get; private set; }
	public Currency Currency { get; private set; }
	public string TxnRef { get; private set; } = null!;
	public string Url { get; private set; } = null!;
	public string? TransactionNo { get; private set; }
	public string? RawResponsePayload { get; private set; }
	public TransactionStatus Status { get; private set; }
	public DateTimeOffset ExpiredAt { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES----------
	// Payment:

	public Payment Payment { get; private set; } = null!;

	// CONSTRUCTORS & FACTORY METHODS
	private Transaction() { }

	private Transaction(
		TransactionId id,
		PaymentId paymentId,
		Provider provider,
		decimal amount,
		Currency currency,
		string txnRef,
		string url,
		TransactionStatus status,
		DateTimeOffset expiredAt,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt,
		string? transactionNo = null,
		string? rawResponsePayload = null
		) : base(id)
	{
		PaymentId = paymentId;
		Provider = provider;
		Amount = amount;
		Currency = currency;
		TxnRef = txnRef;
		Url = url;
		Status = status;
		ExpiredAt = expiredAt;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}



	public static Result<Transaction> Create(
		PaymentId paymentId,
		Provider provider,
		decimal amount,
		Currency currency,
		string txnRef,
		string url,
		TransactionStatus status,
		DateTimeOffset expiredAt,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt
		)
	{
		if (paymentId == null)
			return Result.Failure<Transaction>(TransactionError.InvalidPaymentId);

		if (Enum.IsDefined(typeof(Provider), provider) == false)
			return Result.Failure<Transaction>(TransactionError.InvalidProvider);

		if (amount <= 0)
			return Result.Failure<Transaction>(TransactionError.InvalidAmount);

		if (Enum.IsDefined(typeof(Currency), currency) == false)
			return Result.Failure<Transaction>(TransactionError.InvalidCurrency);

		if (string.IsNullOrEmpty(txnRef))
			return Result.Failure<Transaction>(TransactionError.InvalidTxnRef);

		if (string.IsNullOrEmpty(url))
			return Result.Failure<Transaction>(TransactionError.InvalidUrl);

		if (Enum.IsDefined(typeof(TransactionStatus), status) == false)
			return Result.Failure<Transaction>(TransactionError.InvalidStatus);

		if (expiredAt < createdAt)
			return Result.Failure<Transaction>(TransactionError.InvalidExpiration);

		var transaction = new Transaction(
			TransactionId.Create(),
			paymentId,
			provider,
			amount,
			currency,
			txnRef,
			url,
			status,
			expiredAt,
			createdAt,
			updatedAt
		);

		return Result.Success(transaction);
	}

	public Result UpdateStatus(
		TransactionStatus status,
		DateTimeOffset updatedAt,
		string? transactionNo = null,
		string? rawResponsePayload = null)
	{
		if (Enum.IsDefined(typeof(TransactionStatus), status) == false
			&& TransactionStatusTransition.IsAllowed(Status, status) == false)
			return Result.Failure(TransactionError.InvalidStatus);

		Status = status;

		if (!string.IsNullOrWhiteSpace(transactionNo))
		{
			TransactionNo = transactionNo;
		}

		if (string.IsNullOrWhiteSpace(rawResponsePayload))
		{
			RawResponsePayload = rawResponsePayload;
		}

		UpdatedAt = updatedAt;

		return Result.Success();
	}

	public Result Cancel(DateTimeOffset updatedAt)
	{
		if (TransactionStatusTransition.IsAllowed(Status, TransactionStatus.Cancelled) == false)
		{
			return Result.Failure(TransactionError.InvalidStatusTransition);
		}

		Status = TransactionStatus.Cancelled;

		UpdatedAt = updatedAt;

		return Result.Success();
	} 
}

