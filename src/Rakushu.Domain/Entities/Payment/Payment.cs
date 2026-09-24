using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Payment.PaymentTransaction;
using Rakushu.Domain.Entities.Plan;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.User.Subscription;

namespace Rakushu.Domain.Entities.Payment;

public sealed class Payment : AggregateRoot<PaymentId>
{
	public UserId UserId { get; private set; } = null!;
	public PlanId PlanId { get; private set; } = null!;
	public SubscriptionId? SubscriptionId { get; private set; }
	public string OrderCode { get; private set; } = null!;
	public decimal Amount { get; private set; }
	public string Currency { get; private set; } = "VND";
	public PaymentStatus Status { get; private set; }
	public string? Description { get; private set; }
	public string? QrCodeUrl { get; private set; }
	public DateTimeOffset ExpiresAt { get; private set; }
	public DateTimeOffset? CompletedAt { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	public User.User User { get; private set; } = null!;
	public Plan.Plan Plan { get; private set; } = null!;
	public Subscription? Subscription { get; private set; }

	private readonly List<PaymentTransaction.PaymentTransaction> _transactions = [];
	public IReadOnlyCollection<PaymentTransaction.PaymentTransaction> Transactions => _transactions.AsReadOnly();

	private Payment() { }

	private Payment(
		PaymentId id,
		UserId userId,
		PlanId planId,
		SubscriptionId? subscriptionId,
		string orderCode,
		decimal amount,
		string currency,
		PaymentStatus status,
		DateTimeOffset expiresAt,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt,
		string? description = null,
		string? qrCodeUrl = null) : base(id)
	{
		UserId = userId;
		PlanId = planId;
		SubscriptionId = subscriptionId;
		OrderCode = orderCode;
		Amount = amount;
		Currency = currency;
		Status = status;
		ExpiresAt = expiresAt;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
		Description = description;
		QrCodeUrl = qrCodeUrl;
	}

	public static Result<Payment> Create(
		UserId userId,
		PlanId planId,
		SubscriptionId? subscriptionId,
		string orderCode,
		decimal amount,
		DateTimeOffset expiresAt,
		DateTimeOffset now,
		string currency = "VND",
		string? description = null,
		string? qrCodeUrl = null)
	{
		if (amount <= 0)
		{
			return Result.Failure<Payment>(PaymentErrors.InvalidAmount);
		}

		if (string.IsNullOrWhiteSpace(orderCode))
		{
			return Result.Failure<Payment>(PaymentErrors.OrderCodeNotFound);
		}

		var payment = new Payment(
			PaymentId.Create(),
			userId,
			planId,
			subscriptionId,
			orderCode.Trim(),
			amount,
			currency,
			PaymentStatus.Pending,
			expiresAt,
			now,
			now,
			description,
			qrCodeUrl);

		return Result.Success(payment);
	}

	public void AttachSubscription(SubscriptionId subscriptionId, DateTimeOffset now)
	{
		SubscriptionId = subscriptionId;
		UpdatedAt = now;
	}

	public Result Complete(DateTimeOffset now)
	{
		if (Status == PaymentStatus.Completed)
		{
			return Result.Failure(PaymentErrors.AlreadyProcessed);
		}

		Status = PaymentStatus.Completed;
		CompletedAt = now;
		UpdatedAt = now;

		return Result.Success();
	}

	public Result Cancel(DateTimeOffset now)
	{
		if (Status != PaymentStatus.Pending)
		{
			return Result.Failure(PaymentErrors.CannotCancel);
		}

		Status = PaymentStatus.Canceled;
		UpdatedAt = now;

		foreach (var tx in _transactions.Where(t => t.Status == TransactionStatus.Pending))
		{
			tx.Cancel(now);
		}

		return Result.Success();
	}

	public Result CancelTransaction(PaymentTransactionId transactionId, DateTimeOffset now)
	{
		var tx = _transactions.FirstOrDefault(t => t.Id == transactionId);
		if (tx is null)
		{
			return Result.Failure(PaymentErrors.TransactionNotFound);
		}

		return tx.Cancel(now);
	}

	public Result Expire(DateTimeOffset now)
	{
		if (Status != PaymentStatus.Pending)
		{
			return Result.Failure(PaymentErrors.AlreadyProcessed);
		}

		Status = PaymentStatus.Expired;
		UpdatedAt = now;

		foreach (var tx in _transactions.Where(t => t.Status == TransactionStatus.Pending))
		{
			tx.MarkExpired(now);
		}

		return Result.Success();
	}

	public void MarkFailed(DateTimeOffset now)
	{
		Status = PaymentStatus.Failed;
		UpdatedAt = now;

		foreach (var tx in _transactions.Where(t => t.Status == TransactionStatus.Pending))
		{
			tx.MarkFailed(now);
		}
	}

	public void AddTransaction(PaymentTransaction.PaymentTransaction transaction, DateTimeOffset now)
	{
		_transactions.Add(transaction);
		UpdatedAt = now;
	}
}
