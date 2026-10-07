using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Plan.Entitlement;
using Rakushu.Domain.Entities.User.Subscription.SubscriptionUsage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Feature;

public sealed class Feature : AggregateRoot<FeatureId>
{
	public string Code { get; private set; } = null!;
	public string Name { get; private set; } = null!;
	public string? Description { get; private set; }
	public bool IsActive { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// Entitlements:
	private readonly List<Entitlement> _entitlements = [];
	public IReadOnlyCollection<Entitlement> Entitlements => _entitlements.AsReadOnly();

	// SubscriptionUsages:
	private readonly List<SubscriptionUsage> _subscriptionUsages = [];
	public IReadOnlyCollection<SubscriptionUsage> SubscriptionUsages => _subscriptionUsages.AsReadOnly();

	private Feature() { }

	private Feature(
		FeatureId featureId,
		string code,
		string name,
		bool isActive,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt,
		string? description = null
		) : base(featureId)
	{
		Code = code;
		Name = name;
		IsActive = isActive;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
		Description = description;
	}

	public static Result<Feature> Create(
		string code,
		string name,
		bool isActive,
		DateTimeOffset createdAt,
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
			isActive,
			createdAt,
			createdAt,
			description
			);

		return Result.Success(feature);
	}

	public Result Update(string name, bool isActive, DateTimeOffset updatedAt, string? description = null)
	{
		if (string.IsNullOrWhiteSpace(name) || name.Length > 100)
			return Result.Failure( FeatureErrors.InvalidName);

		Name = name;
		Description = description;
		IsActive = isActive;
		UpdatedAt = updatedAt;

		return Result.Success();
	}

}
