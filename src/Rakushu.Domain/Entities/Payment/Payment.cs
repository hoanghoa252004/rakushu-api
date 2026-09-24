using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Payment.Transaction;
using Rakushu.Domain.Entities.Plan;
using Rakushu.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Rakushu.Domain.Entities.Payment;

public sealed class Payment : AggregateRoot<PaymentId>
{
	public UserId UserId { get; private set; } = null!;
	public PlanId PlanId { get; private set; } = null!;
	public decimal Amount { get; private set; }
	public Currency Currency { get; private set; }
	public PaymentStatus Status { get; private set; }
	public DateTimeOffset ExpiredAt { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES----------
	// User:
	public User.User User { get; private set; } = null!;

	// Plan:
	public Plan.Plan Plan { get; private set; } = null!;

	// Transactions:
	private readonly List<Transaction.Transaction> _transactions = [];
	public IReadOnlyCollection<Transaction.Transaction> Transactions => _transactions.AsReadOnly();

	private Payment(
		PaymentId id,
		UserId userId,
		PlanId planId,
		decimal amount,
		Currency currency,
		PaymentStatus status,
		DateTimeOffset expiredAt,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt
		) : base(id)
	{
		UserId  = userId;
		PlanId = planId;
		Amount = amount;
		Currency = currency;
		Status = status;
		ExpiredAt = expiredAt;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}

	private Payment() { }

	public static Result<Payment> Create(
		UserId userId,
		PlanId planId,
		decimal amount,
		Currency currency,
		PaymentStatus status,
		DateTimeOffset expiredAt,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt
		)
	{
		if (userId == null)
			return Result.Failure<Payment>(PaymentError.InvalidUserId);

		if (planId == null)
			return Result.Failure<Payment>(PaymentError.InvalidPlanId);

		if (amount <= 0)
			return Result.Failure<Payment>(PaymentError.InvalidAmount);

		if (Enum.IsDefined(typeof(Currency), currency) == false)
			return Result.Failure<Payment>(PaymentError.InvalidCurrency);

		if (Enum.IsDefined(typeof(PaymentStatus), status) == false)
			return Result.Failure<Payment>(PaymentError.InvalidStatus);

		if (expiredAt < createdAt)
			return Result.Failure<Payment>(PaymentError.InvalidExpiration);

		var payment = new Payment(
			PaymentId.Create(),
			userId,
			planId,
			amount,
			currency,
			status,
			expiredAt,
			createdAt,
			updatedAt
		);

		return Result.Success(payment);
	}

	public Result UpdateStatus(
		PaymentStatus status,
		DateTimeOffset updatedAt)
	{
		if (Enum.IsDefined(typeof(PaymentStatus), status) == false
			&& PaymentStatusTransition.IsAllowed(Status, status) == false)
			return Result.Failure(PaymentError.InvalidStatus);

		Status = status;

		UpdatedAt = updatedAt;

		return Result.Success();
	}

	public Result AddTransaction(Transaction.Transaction transaction)
	{
		if (transaction.PaymentId != Id)
			return Result.Failure(PaymentError.TransactionNotBelong);

		_transactions.Add(transaction);

		return Result.Success();
	}
}
