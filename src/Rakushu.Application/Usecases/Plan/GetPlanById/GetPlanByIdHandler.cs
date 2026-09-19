using MediatR;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Plan;

namespace Rakushu.Application.Usecases.Plan.GetPlanById;

public sealed class GetPlanByIdHandler : IRequestHandler<GetPlanByIdQuery, Result<PlanDto>>
{
	// DAOs
	private readonly IPlanQuery _planQuery;

	public GetPlanByIdHandler(IPlanQuery planQuery)
	{
		_planQuery = planQuery;
	}

	public async Task<Result<PlanDto>> Handle(GetPlanByIdQuery request, CancellationToken cancellationToken)
	{
		var plan = await _planQuery.GetByIdAsync(
			PlanId.From(request.PlanId),
			cancellationToken);

		if (plan == null)
		{
			return Result.Failure<PlanDto>(PlanErrors.NotFound);
		}

		return Result.Success(plan);
	}
}
