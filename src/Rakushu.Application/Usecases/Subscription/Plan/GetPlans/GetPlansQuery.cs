using MediatR;
using Rakushu.Application.Usecases.Subscription.Plan.GetPlanById;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Subscription.Plan.GetPlans;

public sealed record GetPlansQuery() : IRequest<Result<IReadOnlyCollection<PlanDto>>>;
