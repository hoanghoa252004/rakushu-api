using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Plan.UpdatePlan;

public record UpdatePlanCommand(
	Guid PlanId,
	string Name,
	decimal Price,
	string Currency,
	string BillingCycle,
	string? Description = null
) : IRequest<Result>;
