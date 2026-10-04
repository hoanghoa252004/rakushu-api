using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Plan;

namespace Rakushu.Application.Usecases.Subscription.Plan.GetPlanById;

public sealed record GetPlanByIdQuery(Guid PlanId) : IRequest<Result<PlanDto>>;
