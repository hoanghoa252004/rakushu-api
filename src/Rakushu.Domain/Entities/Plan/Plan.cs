using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Feature;
using Rakushu.Domain.Entities.Payment;
using Rakushu.Domain.Entities.Plan.Entitlement;
using Rakushu.Domain.Entities.User.Subscription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Plan;

public class Plan : AggregateRoot<PlanId>
{
	public string Code { get; private set; } = null!;
	public string Name { get; private set; } = null!;
	public string JapaneseName { get; private set; } = null!;
	public string? Description { get; private set; }
	public decimal Price { get; private set; }
	public Currency Currency { get; private set; }
	public BillingCycle BillingCycle { get; private set; }
	public bool IsActive { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// Entitlements:
	private readonly List<Entitlement.Entitlement> _entitlements = new List<Entitlement.Entitlement>();
	public IReadOnlyCollection<Entitlement.Entitlement> Entitlements => _entitlements.AsReadOnly();

	// Subscriptions:
	private readonly List<Subscription> _subscriptions = [];
	public IReadOnlyCollection<Subscription> Subscriptions => _subscriptions.AsReadOnly();

	// Payments:
	private readonly List<Payment.Payment> _payments = [];
	public IReadOnlyCollection<Payment.Payment> Payments => _payments.AsReadOnly();

	private Plan() { }

	private Plan(
		PlanId planId,
		string planCode,
		string name,
		string japaneseName,
		decimal price,
		Currency currency,
		BillingCycle billingCycle,
		bool isActive,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt,
		string? description = null
	) : base(planId)
	{
		Name = name;
		JapaneseName = japaneseName;
		Code = planCode;
		Price = price;
		Currency = currency;
		BillingCycle = billingCycle;
		IsActive = isActive;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
		Description = description;
	}

	public static Result<Plan> Create(
		string planCode,
		string name,
		string japaneseName,
		decimal price,
		Currency currency,
		BillingCycle billingCycle,
		bool isActive,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt,
	string? description = null)
	{
		if (string.IsNullOrWhiteSpace(name) || name.Length > 100)
			return Result.Failure<Plan>(PlanErrors.InvalidName);

		if (string.IsNullOrWhiteSpace(japaneseName) || japaneseName.Length > 100)
			return Result.Failure<Plan>(PlanErrors.InvalidJapaneseName);

		if (price < 0)
			return Result.Failure<Plan>(PlanErrors.InvalidPrice);

		if (!Enum.IsDefined(currency))
			return Result.Failure<Plan>(PlanErrors.InvalidCurrency);

		if (!Enum.IsDefined(billingCycle))
			return Result.Failure<Plan>(PlanErrors.InvalidBillingCycle);

		var plan = new Plan(
			PlanId.Create(),
			planCode,
			name,
			japaneseName,
			price,
			currency,
			billingCycle,
			isActive,
			createdAt,
			updatedAt,
			description
		);

		return Result.Success(plan);
	}

	public Result Update(
		string name,
		string japaneseName,
		decimal price,
		Currency currency,
		BillingCycle billingCycle,
		bool isActive,
		DateTimeOffset updatedAt,
		string? description = null)
	{
		if (string.IsNullOrWhiteSpace(name) || name.Length > 100)
			return Result.Failure<Plan>(PlanErrors.InvalidName);

		if (string.IsNullOrWhiteSpace(japaneseName) || japaneseName.Length > 100)
			return Result.Failure<Plan>(PlanErrors.InvalidJapaneseName);

		if (price < 0)
			return Result.Failure<Plan>(PlanErrors.InvalidPrice);

		if (!Enum.IsDefined(currency))
			return Result.Failure(PlanErrors.InvalidCurrency);

		if (!Enum.IsDefined(billingCycle))
			return Result.Failure(PlanErrors.InvalidBillingCycle);

		Name = name;
		Price = price;
		Currency = currency;
		BillingCycle = billingCycle;
		IsActive = isActive;
		Description = description;
		JapaneseName = japaneseName;
		UpdatedAt = updatedAt;

		return Result.Success();
	}

	public Result<Entitlement.Entitlement> AddEntitlement(
		FeatureId featureId,
		bool isEnabled,
		LimitUnit limitUnit,
		int limitValue,
		LimitPeriod limitPeriod,
		DateTimeOffset updatedAt)
	{
		if (!IsActive)
			return Result.Failure<Entitlement.Entitlement>(EntitlementErrors.PlanNotActive);

		if (_entitlements.Any(e => e.FeatureId == featureId))
			return Result.Failure<Entitlement.Entitlement>(EntitlementErrors.DuplicateFeature);

		var entitlementResult = Entitlement.Entitlement.Create(
			Id,
			featureId,
			isEnabled,
			limitUnit,
			limitValue,
			limitPeriod
		);

		if (entitlementResult.IsFailure)
			return entitlementResult;

		_entitlements.Add(entitlementResult.Value);

		UpdatedAt = updatedAt;

		return entitlementResult;
	}

	public Result UpdateEntitlement(
		EntitlementId entitlementId,
		bool isEnabled,
		LimitUnit limitUnit,
		int limitValue,
		LimitPeriod limitPeriod,
		DateTimeOffset updatedAt)
	{
		if (!IsActive)
			return Result.Failure(EntitlementErrors.PlanNotActive);

		var entitlement = _entitlements.FirstOrDefault(e => e.Id == entitlementId);

		if (entitlement is null)
			return Result.Failure(EntitlementErrors.NotFound);

		var updateResult = entitlement.Update(isEnabled, limitUnit, limitValue, limitPeriod);

		if (updateResult.IsFailure)
		{
			return updateResult;
		}

		UpdatedAt = updatedAt;

		return Result.Success();
	}

	public Result RemoveEntitlement(EntitlementId entitlementId, DateTimeOffset updatedAt)
	{

		if (_subscriptions.Any(s => s.Status == SubscriptionStatus.Active))
		{
			return Result.Failure(EntitlementErrors.CannotDeleteEntitlementWithSubscriptions);
		}

		var entitlement = _entitlements.FirstOrDefault(e => e.Id == entitlementId);

		if (entitlement is null)
		{
			return Result.Failure(EntitlementErrors.NotFound);
		}

		_entitlements.Remove(entitlement);

		UpdatedAt = updatedAt;

		return Result.Success();
	}
}
