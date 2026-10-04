namespace Rakushu.Application.Usecases.Subscription.Plan.GetPlanById;

public sealed record PlanDto(
	Guid Id,
	string Code,
	string Name,
	decimal Price,
	string Currency,
	string BillingCycle,
	string Status,
	DateTime CreatedAt,
	DateTime UpdatedAt,
	string? Description = null
);
