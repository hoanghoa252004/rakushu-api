using MediatR;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Plan.GetPlanById;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Plan.GetPlans;

public sealed class GetPlansHandler : IRequestHandler<GetPlansQuery, Result<PaginatedList<PlanDto>>>
{
	private readonly IPlanQuery _planQuery;

	public GetPlansHandler(IPlanQuery planQuery)
	{
		_planQuery = planQuery;
	}

	public async Task<Result<PaginatedList<PlanDto>>> Handle(GetPlansQuery request, CancellationToken cancellationToken)
	{
		var(items, totalCount) = await _planQuery.GetPlansAsync(request, cancellationToken);

		var result = PaginatedList<PlanDto>.Create(
			items,
			totalCount,
			request.PageNumber,
			request.PageSize
			);

		return Result.Success(result);
	}
}
