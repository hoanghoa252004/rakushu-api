using MediatR;
using Rakushu.Application.Usecases.Subscription.Entitlement;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Subscription.Entitlement.GetEntitlements;

public sealed record GetEntitlementsQuery(Guid PlanId) : IRequest<Result<IReadOnlyCollection<EntitlementDto>>>;
