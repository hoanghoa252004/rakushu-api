using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Feature.ObjectValues;
using Rakushu.Domain.Entities.Plan.PlanEntitlement;
using Rakushu.Domain.Entities.User.Subscription.SubscriptionUsage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Feature;

public sealed class Feature : AggregateRoot<FeatureId>
{
	public FeatureCode Code { get; private set; } = null!;
	public string Name { get; private set; } = null!;
	public string? Description { get; private set; }
	public FeatureStatus Status { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// PlanEntitlements:
	private readonly List<PlanEntitlement> _planEntitlements = [];
	public IReadOnlyCollection<PlanEntitlement> PlanEntitlements => _planEntitlements.AsReadOnly();

	// SubscriptionUsages:
	private readonly List<SubscriptionUsage> _subscriptionUsages = [];
	public IReadOnlyCollection<SubscriptionUsage> SubscriptionUsages => _subscriptionUsages.AsReadOnly();

	private Feature() { }

	private Feature(
		FeatureId featureId,
		FeatureCode code,
		string name,
		FeatureStatus status,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt,
		string? description = null
		) : base(featureId)
	{
		Code = code;
		Name = name;
		Status = status;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
		Description = description;
	}

	public static Result<Feature> Create(
		FeatureCode code,
		string name,
		FeatureStatus status,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt,
		string? description = null
		)
	{
		if (string.IsNullOrWhiteSpace(name) || name.Length > 50)
			return Result.Failure<Feature>(
				FeatureErrors.InvalidName);

		var feature = new Feature(
			FeatureId.Create(),
			code,
			name,
			status,
			createdAt,
			updatedAt,
			description
			);

		return Result.Success(feature);
	}

	public Result Update(string name, string? description, DateTimeOffset updatedAt)
	{
		if (string.IsNullOrWhiteSpace(name) || name.Length > 50)
			return Result.Failure(
				FeatureErrors.InvalidName);

		Name = name;
		Description = description;
		UpdatedAt = updatedAt;

		return Result.Success();
	}

	public Result ChangeStatus(FeatureStatus status)
	{
		if (!FeatureStatusTransition.IsAllowed(Status, status))
		{
			return Result.Failure(CommonError.InvalidStatusTransition);
		}

		Status = status;

		return Result.Success();
	}
}
