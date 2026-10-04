using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Subscription.Entitlement.DeleteEntitlement;

public sealed record DeleteEntitlementCommand(
	Guid PlanId,
	Guid EntitlementId
) : IRequest<Result>;
