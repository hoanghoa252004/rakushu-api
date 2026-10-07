using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Feature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.User.Subscription.SubscriptionUsage;

public sealed class SubscriptionUsage : Entity<SubscriptionUsageId>
{
	public SubscriptionId SubscriptionId { get; private set; } = null!;
	public FeatureId FeatureId { get; private set; } = null!;
	public DateTimeOffset PeriodStart { get; private set; }
	public DateTimeOffset PeriodEnd { get; private set; }
	public int MaxValue { get; private set; }
	public int UsedValue { get; private set; }
	public bool IsOverLimit { get; private set; }
	public bool IsExpired { get; private set; }
	public bool IsCanceled { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// Subscription
	public Subscription Subscription { get; private set; } = null!;

	// Feature 
	public Feature.Feature Feature { get; private set;  } = null!;

	private SubscriptionUsage() { }

	private SubscriptionUsage(
		SubscriptionUsageId subscriptionUsageId,
		SubscriptionId subscriptionId,
		FeatureId featureId,
		DateTimeOffset periodStart,
		DateTimeOffset periodEnd,
		int maxValue,
		int usedValue,
		bool isOverLimit,
		bool isExpired,
		bool isCanceled,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt
		) : base(subscriptionUsageId)
	{
		SubscriptionId = subscriptionId;
		FeatureId = featureId;
		PeriodStart = periodStart;
		PeriodEnd = periodEnd;
		MaxValue = maxValue;
		UsedValue = usedValue;
		IsOverLimit = isOverLimit;
		IsExpired = isExpired;
		IsCanceled = isCanceled;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}

	public static Result<SubscriptionUsage> Create(
		SubscriptionId subscriptionId,
		FeatureId featureId,
		DateTimeOffset periodStart,
		DateTimeOffset periodEnd,
		int maxValue,
		int usedValue,
		bool isOverLimit,
		bool isExpired,
		bool isCanceled,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt
		)
	{
		var subscriptionUsage = new SubscriptionUsage(
			SubscriptionUsageId.Create(),
			subscriptionId,
			featureId,
			periodStart,
			periodEnd,
			maxValue,
			usedValue,
			isOverLimit,
			isExpired,
			isCanceled,
			createdAt,
			updatedAt
			);

		return Result.Success(subscriptionUsage);
	}

	public Result Expire(DateTimeOffset now)
	{
		if (IsExpired == true)
			return Result.Failure(SubscriptionUsageErrors.AlreadyExpired);

		IsExpired = true;
		UpdatedAt = now;

		return Result.Success();
	}

	public Result Cancel(DateTimeOffset now)
	{
		if (IsCanceled == true)
			return Result.Failure(SubscriptionUsageErrors.AlreadyCanceled);

		IsCanceled = true;
		UpdatedAt = now;

		return Result.Success();
	}
}
