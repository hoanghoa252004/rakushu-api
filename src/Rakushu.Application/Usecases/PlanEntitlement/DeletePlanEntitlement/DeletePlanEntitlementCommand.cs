using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.PlanEntitlement.DeletePlanEntitlement;

public sealed record DeletePlanEntitlementCommand(
	Guid PlanId,
	Guid EntitlementId
) : IRequest<Result>;
