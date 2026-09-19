using MediatR;
using Rakushu.Application.Usecases.Admin.Users.ChangeUserStatus;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Plan;
using Rakushu.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Plan.ChangePlanStatus;

internal sealed class ChangePlanStatusHandler : IRequestHandler<ChangePlanStatusCommand, Result>
{
	// DAOs
	private readonly IPlanRepository _planRepository;

	// UNIT OF WORK
	private readonly IUnitOfWork _unitOfWork;

	public ChangePlanStatusHandler(
		IPlanRepository planRepository, 
		IUnitOfWork unitOfWork
		)
	{
		_planRepository = planRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(ChangePlanStatusCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var planId = PlanId.From(request.PlanId);

			var plan = await _planRepository.GetByIdAsync(planId, cancellationToken);

			if (plan == null)
			{
				return Result.Failure(PlanErrors.NotFound);
			}

			return plan.ChangeStatus(request.Status);
		}, cancellationToken);
	}
}
