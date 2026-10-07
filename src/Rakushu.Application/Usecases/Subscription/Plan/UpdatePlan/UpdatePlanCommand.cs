using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Subscription.Plan.UpdatePlan;

public record UpdatePlanCommand(
	Guid PlanId,
	string Name,
	string JapaneseName,
	decimal Price,
	string Currency,
	string BillingCycle,
	bool IsActive,
	string? Description = null
) : IRequest<Result>;
