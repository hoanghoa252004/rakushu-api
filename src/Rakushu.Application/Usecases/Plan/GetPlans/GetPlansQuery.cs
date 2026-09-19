using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Plan.GetPlanById;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Plan;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Application.Usecases.Plan.GetPlans;

public record GetPlansQuery(
	int PageNumber = 1,
	int PageSize = 10,
	IReadOnlyCollection<Guid>? FeatureIds = null,
	PlanStatus? Status = null
	) : IRequest<Result<PaginatedList<PlanDto>>>;
