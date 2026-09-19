using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Plan.GetPlanById;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Plan;
using Rakushu.Domain.Entities.Role;
using Rakushu.Domain.Entities.User;
using System.Numerics;

namespace Rakushu.Application.Usecases.Plan.GetPlans;

public sealed class GetPlansHandler : IRequestHandler<GetPlansQuery, Result<PaginatedList<PlanDto>>>
{
	// USER CONTEXT
	private readonly ICurrentUserContext _currentUserContext;

	// QUERY
	private readonly IPlanQuery _planQuery;

	public GetPlansHandler(
		ICurrentUserContext currentUserContext,
		IPlanQuery planQuery)
	{
		_currentUserContext = currentUserContext;
		_planQuery = planQuery;
	}

	public async Task<Result<PaginatedList<PlanDto>>> Handle(GetPlansQuery request, CancellationToken cancellationToken)
	{
		// Parse status
		if (string.IsNullOrWhiteSpace(request.Status) == false && !Enum.TryParse<PlanStatus>(request.Status, true, out var status))
		{
			return Result.Failure<PaginatedList<PlanDto>>(PlanErrors.InvalidStatus);
		}

		var (items, totalCount) = await _planQuery.GetPlansAsync(request, cancellationToken);

		// Authorize resource
		var isAdmin =
			Enum.TryParse<DefaultSystemRoles>(_currentUserContext.Role, true, out var role)
			&& role == DefaultSystemRoles.SystemAdministrator;

		var filteredByRolePlans = !isAdmin
			? items.Where(p => p.Status == PlanStatus.Active.ToString()).ToList()
			: items.ToList();

		var result = PaginatedList<PlanDto>.Create(
			filteredByRolePlans,
			totalCount,
			request.PageNumber,
			request.PageSize
			);

		return Result.Success(result);
	}
}
