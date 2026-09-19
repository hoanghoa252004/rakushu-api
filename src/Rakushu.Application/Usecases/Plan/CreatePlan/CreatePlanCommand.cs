using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Plan.CreatePlan;

public record CreatePlanCommand(
	string Code,
	string Name,
	decimal Price,
	string Currency,
	string BillingCycle,
	string? Description = null
) : IRequest<Result<Guid>>;
