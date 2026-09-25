using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.PlanEntitlement.GetPlanEntitlements;

public sealed record GetPlanEntitlementsQuery(
	Guid PlanId
) : IRequest<Result<IReadOnlyCollection<PlanEntitlementDto>>>;
