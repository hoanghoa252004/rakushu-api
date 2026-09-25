using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Feature;
using Rakushu.Domain.Entities.Payment;
using Rakushu.Domain.Entities.Plan.ObjectValues;
using Rakushu.Domain.Entities.Plan.PlanEntitlement;
using Rakushu.Domain.Entities.User.Subscription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Plan;

public class Plan : AggregateRoot<PlanId>
{
	public PlanCode Code { get; private set; } = null!;
	public string Name { get; private set; } = null!;
	public string? Description { get; private set;  }
	public decimal Price { get; private set; }
	public Currency Currency { get; private set; } 
	public BillingCycle BillingCycle { get; private set; }
	public PlanStatus Status { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// PlanEntitlements:
	private readonly List<PlanEntitlement.PlanEntitlement> _planEntitlements = [];
	public IReadOnlyCollection<PlanEntitlement.PlanEntitlement> PlanEntitlements => _planEntitlements.AsReadOnly();

	// Subscriptions:
	private readonly List<Subscription> _subscriptions = [];
	public IReadOnlyCollection<Subscription> Subscriptions => _subscriptions.AsReadOnly();

	// Payments:
	private readonly List<Payment.Payment> _payments = [];
	public IReadOnlyCollection<Payment.Payment> Payments => _payments.AsReadOnly();

	private Plan() { }

	private Plan(
		PlanId planId,
		PlanCode planCode,
		string name,
		decimal price,
		Currency currency,
		BillingCycle billingCycle,
		PlanStatus status,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt,
		string? description = null
		) : base(planId)
	{
		Name = name;
		Code = planCode;
		Price = price;
		Currency = currency;
		BillingCycle = billingCycle;
		Status = status;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
		Description = description;
	}

	public static Result<Plan> Create(
	PlanCode planCode,
	string name,
	decimal price,
	Currency currency,
	BillingCycle billingCycle,
	PlanStatus status,
	DateTimeOffset createdAt,
	DateTimeOffset updatedAt,
	string? description = null)
	{
		if (string.IsNullOrWhiteSpace(name) || name.Length > 50)
			return Result.Failure<Plan>(
				PlanErrors.InvalidName);

		if (price <= 0)
			return Result.Failure<Plan>(
				PlanErrors.InvalidPrice);

		if (!Enum.IsDefined(currency))
			return Result.Failure<Plan>(
				PlanErrors.InvalidCurrency);

		if (!Enum.IsDefined(billingCycle))
			return Result.Failure<Plan>(
				PlanErrors.InvalidBillingCycle);

		if (!Enum.IsDefined(status))
			return Result.Failure<Plan>(
				PlanErrors.InvalidStatus);

		var plan = new Plan(
			PlanId.Create(),
			planCode,
			name,
			price,
			currency,
			billingCycle,
			status,
			createdAt,
			updatedAt,
			description
		);

		return Result.Success(plan);
	}

	public Result Update(
	string name,
	decimal price,
	Currency currency,
	BillingCycle billingCycle,
	DateTimeOffset updatedAt,
	string? description = null)
	{
		if (string.IsNullOrWhiteSpace(name) || name.Length > 50)
			return Result.Failure(
				PlanErrors.InvalidName);

		if (price <= 0)
			return Result.Failure(
				PlanErrors.InvalidPrice);

		if (!Enum.IsDefined(currency))
			return Result.Failure(
				PlanErrors.InvalidCurrency);

		if (!Enum.IsDefined(billingCycle))
			return Result.Failure(
				PlanErrors.InvalidBillingCycle);

		Name = name;
		Price = price;
		Currency = currency;
		BillingCycle = billingCycle;
		Description = description;
		UpdatedAt = updatedAt;

		return Result.Success();
	}

	public Result ChangeStatus(PlanStatus status)
	{
		if (!PlanStatusTransition.IsAllowed(Status, status))
		{
			return Result.Failure(CommonError.InvalidStatusTransition);
		}

		Status = status;

		return Result.Success();
	}

	public Result<PlanEntitlement.PlanEntitlement> AddEntitlement(
		FeatureId featureId,
		bool isEnabled,
		int limitValue,
		LimitUnit limitUnit,
		LimitPeriod limitPeriod,
		DateTimeOffset updatedAt)
	{
		if (Status == PlanStatus.Archived)
		{
			return Result.Failure<PlanEntitlement.PlanEntitlement>(PlanEntitlementErrors.PlanArchived);
		}

		if (_planEntitlements.Any(pe => pe.FeatureId == featureId))
		{
			return Result.Failure<PlanEntitlement.PlanEntitlement>(PlanEntitlementErrors.DuplicateFeature);
		}

		var entitlementResult = PlanEntitlement.PlanEntitlement.Create(
			Id,
			featureId,
			isEnabled,
			limitValue,
			limitUnit,
			limitPeriod);

		if (entitlementResult.IsFailure)
		{
			return entitlementResult;
		}

		_planEntitlements.Add(entitlementResult.Value);
		UpdatedAt = updatedAt;

		return entitlementResult;
	}

	public Result UpdateEntitlement(
		PlanEntitlementId entitlementId,
		bool isEnabled,
		int limitValue,
		LimitUnit limitUnit,
		LimitPeriod limitPeriod,
		DateTimeOffset updatedAt)
	{
		if (Status == PlanStatus.Archived)
		{
			return Result.Failure(PlanEntitlementErrors.PlanArchived);
		}

		var entitlement = _planEntitlements.FirstOrDefault(pe => pe.Id == entitlementId);
		if (entitlement is null)
		{
			return Result.Failure(PlanEntitlementErrors.NotFound);
		}

		var updateResult = entitlement.Update(isEnabled, limitValue, limitUnit, limitPeriod);
		if (updateResult.IsFailure)
		{
			return updateResult;
		}

		UpdatedAt = updatedAt;

		return Result.Success();
	}

	public Result RemoveEntitlement(PlanEntitlementId entitlementId, DateTimeOffset updatedAt)
	{
		if (Status == PlanStatus.Archived)
		{
			return Result.Failure(PlanEntitlementErrors.PlanArchived);
		}

		if (_subscriptions.Any(s => s.Status == SubscriptionStatus.Active))
		{
			return Result.Failure(PlanEntitlementErrors.CannotDeleteEntitlementWithSubscriptions);
		}

		var entitlement = _planEntitlements.FirstOrDefault(pe => pe.Id == entitlementId);
		if (entitlement is null)
		{
			return Result.Failure(PlanEntitlementErrors.NotFound);
		}

		_planEntitlements.Remove(entitlement);
		UpdatedAt = updatedAt;

		return Result.Success();
	}
}
