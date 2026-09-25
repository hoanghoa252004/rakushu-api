using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.PlanEntitlement.AddPlanEntitlement;

public sealed record AddPlanEntitlementCommand(
	Guid PlanId,
	Guid FeatureId,
	bool IsEnabled,
	int LimitValue,
	string LimitUnit,
	string LimitPeriod
) : IRequest<Result<Guid>>;
