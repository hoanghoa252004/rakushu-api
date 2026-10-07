using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Feature;
using Rakushu.Domain.Entities.Payment;
using Rakushu.Domain.Entities.Plan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.User.Subscription;

public sealed class Subscription : Entity<SubscriptionId>
{
	public UserId UserId { get; private set; } = null!;
	public PaymentId? PaymentId { get; private set; } = null!;
	public PlanId PlanId { get; private set; } = null!;
	public SubscriptionStatus Status { get; private set; }
	public DateTimeOffset StartAt { get; private set; }
	public DateTimeOffset EndAt { get; private set; }

	// NAVIGATION PROPERTIES
	// User
	public User User { get; private set; } = null!;

	// Payment
	public Payment.Payment? Payment { get; private set; }

	// Plan
	public Plan.Plan Plan { get; private set; } = null!;

	// SubscriptionUsages:
	private readonly List<SubscriptionUsage.SubscriptionUsage> _subscriptionUsages = [];
	public IReadOnlyCollection<SubscriptionUsage.SubscriptionUsage> SubscriptionUsages => _subscriptionUsages.AsReadOnly();

	private Subscription() { }

	private Subscription(
		SubscriptionId subscriptionId,
		UserId userId,
		PlanId planId,
		SubscriptionStatus status,
		DateTimeOffset startAt,
		DateTimeOffset endAt,
		PaymentId? paymentId = null
		) : base(subscriptionId)
	{
		UserId = userId;
		PaymentId = paymentId;
		PlanId = planId;
		Status = status;
		StartAt = startAt;
		EndAt = endAt;
	}

	public static Result<Subscription> Create(
		UserId userId,
		PlanId planId,
		SubscriptionStatus status,
		DateTimeOffset startAt,
		DateTimeOffset endAt,
		PaymentId? paymentId = null
		)
	{
		var subscription = new Subscription(
			SubscriptionId.Create(),
			userId,
			planId,
			status,
			startAt,
			endAt,
			paymentId
			);

		return Result.Success(subscription);
	}

	public Result<SubscriptionUsage.SubscriptionUsage> AddUsage(
		FeatureId featureId,
		DateTimeOffset periodStart,
		DateTimeOffset periodEnd,
		int maxValue,
		int usedValue,
		bool isOverLimit,
		bool isExpired,
		bool isCanceled,
		DateTimeOffset now
		)
	{
		var usageResult = SubscriptionUsage.SubscriptionUsage.Create(
			Id,
			featureId,
			periodStart,
			periodEnd,
			maxValue,
			usedValue,
			isOverLimit,
			isExpired,
			isCanceled,
			now,
			now
			);

		if (usageResult.IsFailure)
			return Result.Failure<SubscriptionUsage.SubscriptionUsage>(usageResult.Error);

		var usage = usageResult.Value;

		_subscriptionUsages.Add(usage);

		return Result.Success(usage);
	}

	public Result Expire(DateTimeOffset now)
	{
		if (Status == SubscriptionStatus.Expired)
			return Result.Failure(SubscriptionErrors.AlreadyExpired);

		Status = SubscriptionStatus.Expired;
		EndAt = now;

		return Result.Success();
	}

	public Result Cancel(DateTimeOffset now)
	{
		if (Status == SubscriptionStatus.Canceled)
			return Result.Failure(SubscriptionErrors.AlreadyCanceled);

		Status = SubscriptionStatus.Canceled;
		EndAt = now;

		return Result.Success();
	}
}
