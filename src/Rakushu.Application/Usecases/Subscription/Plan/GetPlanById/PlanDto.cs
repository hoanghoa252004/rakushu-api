using Rakushu.Application.Usecases.Subscription.Entitlement.GetEntitlements;

namespace Rakushu.Application.Usecases.Subscription.Plan.GetPlanById;

public sealed record PlanDto(
	Guid Id,
	string Code,
	string Name,
	string JapaneseName,
	decimal Price,
	string Currency,
	string BillingCycle,
	bool IsActive,
	DateTimeOffset CreatedAt,
	DateTimeOffset UpdatedAt,
	IReadOnlyCollection<EntitlementDto> Entitlements,
	string? Description = null
);

public sealed record PlanBasicDto(
	Guid Id,
	string Code,
	string Name,
	string JapaneseName,
	decimal Price,
	string Currency,
	string BillingCycle,
	bool IsActive,
	string? Description
);