using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Subscription.Plan.CreatePlan;

public record CreatePlanCommand(
	string Code,
	string Name,
	string JapaneseName,
	decimal Price,
	string Currency,
	string BillingCycle,
	bool IsActive,
	string? Description = null
) : IRequest<Result<Guid>>;
