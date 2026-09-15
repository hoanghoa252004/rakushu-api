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
	public int UsedValue { get; private set; }
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
		int usedValue,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt
		) : base(subscriptionUsageId)
	{
		SubscriptionId = subscriptionId;
		FeatureId = featureId;
		PeriodStart = periodStart;
		PeriodEnd = periodEnd;
		UsedValue = usedValue;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}

	public static Result<SubscriptionUsage> Create(
		SubscriptionId subscriptionId,
		FeatureId featureId,
		DateTimeOffset periodStart,
		DateTimeOffset periodEnd,
		int usedValue,
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
			usedValue,
			createdAt,
			updatedAt
			);

		return Result.Success(subscriptionUsage);
	}
}
