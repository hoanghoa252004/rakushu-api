using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Subscription.Plan.GetPlanById;

public sealed record GetPlanByIdQuery(Guid PlanId) : IRequest<Result<PlanDto>>;
