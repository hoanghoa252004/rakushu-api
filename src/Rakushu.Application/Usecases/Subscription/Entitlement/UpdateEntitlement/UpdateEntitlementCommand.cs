using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Subscription.Entitlement.UpdateEntitlement;

public sealed record UpdateEntitlementCommand(
	Guid PlanId,
	Guid EntitlementId,
	bool IsEnabled,
	string LimitUnit,
	int LimitValue,
	string LimitPeriod
) : IRequest<Result>;
