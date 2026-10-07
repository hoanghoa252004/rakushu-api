using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Subscription.Entitlement.CreateEntitlement;

public sealed record CreateEntitlementCommand(
	Guid PlanId,
	Guid FeatureId,
	bool IsEnabled,
	string LimitUnit,
	int LimitValue,
	string LimitPeriod
) : IRequest<Result<Guid>>;
