namespace Rakushu.Application.Usecases.Plan.GetPlanById;

public sealed record PlanDto(
	Guid Id,
	string Code,
	string Name,
	decimal Price,
	string Currency,
	string BillingCycle,
	string Status,
	DateTimeOffset CreatedAt,
	DateTimeOffset UpdatedAt,
	string? Description = null
);
