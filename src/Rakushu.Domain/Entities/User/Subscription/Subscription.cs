using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
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
	public PlanId PlanId { get; private set; } = null!;
	public DateTimeOffset StartDate { get; private set; }
	public SubscriptionStatus Status { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// MAVOGATION PROPERTIES
	// User
	public User User { get; private set; } = null!;

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
		DateTimeOffset startDate,
		SubscriptionStatus status,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt
		) : base(subscriptionId)
	{
		UserId = userId;
		PlanId = planId;
		StartDate = startDate;
		Status = status;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}

	public static Result<Subscription> Create(
		UserId userId,
		PlanId planId,
		DateTimeOffset startDate,
		SubscriptionStatus status,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt
		)
	{
		var subscription = new Subscription(
			SubscriptionId.Create(),
			userId,
			planId,
			startDate,
			status,
			createdAt,
			updatedAt
			);

		return Result.Success(subscription);
	}
}
