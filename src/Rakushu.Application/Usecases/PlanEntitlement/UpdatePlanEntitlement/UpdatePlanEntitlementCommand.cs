using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.PlanEntitlement.UpdatePlanEntitlement;

public sealed record UpdatePlanEntitlementCommand(
	Guid PlanId,
	Guid EntitlementId,
	bool IsEnabled,
	int LimitValue,
	string LimitUnit,
	string LimitPeriod
) : IRequest<Result>;
