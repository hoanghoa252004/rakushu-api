using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Plan.DeletePlan;

public sealed record DeletePlanCommand(Guid PlanId) : IRequest<Result>;
