using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Subscription.Entitlement.AddEntitlement;

public sealed record AddEntitlementCommand(
	Guid PlanId,
	Guid FeatureId,
	bool IsEnabled,
	int LimitValue,
	string LimitUnit,
	string LimitPeriod
) : IRequest<Result<Guid>>;
