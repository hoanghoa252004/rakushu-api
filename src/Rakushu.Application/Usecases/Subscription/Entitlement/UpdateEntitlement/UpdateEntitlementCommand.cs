using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Subscription.Entitlement.UpdateEntitlement;

public sealed record UpdateEntitlementCommand(
	Guid PlanId,
	Guid EntitlementId,
	bool IsEnabled,
	int LimitValue,
	string LimitUnit,
	string LimitPeriod
) : IRequest<Result>;
