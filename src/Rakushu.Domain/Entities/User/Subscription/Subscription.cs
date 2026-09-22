using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Plan;

namespace Rakushu.Domain.Entities.User.Subscription;

public sealed class Subscription : Entity<SubscriptionId>
{
	public UserId UserId { get; private set; } = null!;
	public PlanId PlanId { get; private set; } = null!;
	public SubscriptionStatus Status { get; private set; }
	public DateTimeOffset StartDate { get; private set; }
	public DateTimeOffset EndDate { get; private set; }
	public DateTimeOffset CurrentPeriodStart { get; private set; }
	public DateTimeOffset CurrentPeriodEnd { get; private set; }
	public DateTimeOffset? CanceledAt { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	public User User { get; private set; } = null!;
	public Plan.Plan Plan { get; private set; } = null!;

	private readonly List<SubscriptionUsage.SubscriptionUsage> _subscriptionUsages = [];
	public IReadOnlyCollection<SubscriptionUsage.SubscriptionUsage> SubscriptionUsages => _subscriptionUsages.AsReadOnly();

	private readonly List<Payment.Payment> _payments = [];
	public IReadOnlyCollection<Payment.Payment> Payments => _payments.AsReadOnly();

	private Subscription() { }

	private Subscription(
		SubscriptionId subscriptionId,
		UserId userId,
		PlanId planId,
		SubscriptionStatus status,
		DateTimeOffset startDate,
		DateTimeOffset endDate,
		DateTimeOffset currentPeriodStart,
		DateTimeOffset currentPeriodEnd,
		DateTimeOffset? canceledAt,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt) : base(subscriptionId)
	{
		UserId = userId;
		PlanId = planId;
		Status = status;
		StartDate = startDate;
		EndDate = endDate;
		CurrentPeriodStart = currentPeriodStart;
		CurrentPeriodEnd = currentPeriodEnd;
		CanceledAt = canceledAt;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}

	public static Result<Subscription> CreatePending(
		UserId userId,
		PlanId planId,
		DateTimeOffset now)
	{
		var subscription = new Subscription(
			SubscriptionId.Create(),
			userId,
			planId,
			SubscriptionStatus.Pending,
			now,
			now,
			now,
			now,
			null,
			now,
			now);

		return Result.Success(subscription);
	}

	public static Result<Subscription> Create(
		UserId userId,
		PlanId planId,
		SubscriptionStatus status,
		DateTimeOffset startDate,
		DateTimeOffset endDate,
		DateTimeOffset now)
	{
		var subscription = new Subscription(
			SubscriptionId.Create(),
			userId,
			planId,
			status,
			startDate,
			endDate,
			startDate,
			endDate,
			null,
			now,
			now);

		return Result.Success(subscription);
	}

	public Result Activate(DateTimeOffset startDate, DateTimeOffset endDate, DateTimeOffset now)
	{
		if (Status != SubscriptionStatus.Pending && Status != SubscriptionStatus.Expired && Status != SubscriptionStatus.Failed)
		{
			return Result.Failure(SubscriptionErrors.CannotActivate);
		}

		Status = SubscriptionStatus.Active;
		StartDate = startDate;
		EndDate = endDate;
		CurrentPeriodStart = startDate;
		CurrentPeriodEnd = endDate;
		CanceledAt = null;
		UpdatedAt = now;

		return Result.Success();
	}

	public Result Cancel(DateTimeOffset now)
	{
		if (Status != SubscriptionStatus.Active)
		{
			return Result.Failure(SubscriptionErrors.CannotCancel);
		}

		Status = SubscriptionStatus.Canceled;
		CanceledAt = now;
		UpdatedAt = now;

		return Result.Success();
	}

	public void MarkAsFailed(DateTimeOffset now)
	{
		Status = SubscriptionStatus.Failed;
		UpdatedAt = now;
	}

	public void Expire(DateTimeOffset now)
	{
		Status = SubscriptionStatus.Expired;
		UpdatedAt = now;
	}

	public void AddUsage(SubscriptionUsage.SubscriptionUsage usage, DateTimeOffset now)
	{
		_subscriptionUsages.Add(usage);
		UpdatedAt = now;
	}
}
