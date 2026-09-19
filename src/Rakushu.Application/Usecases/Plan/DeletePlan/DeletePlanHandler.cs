using MediatR;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Plan;

namespace Rakushu.Application.Usecases.Plan.DeletePlan;

public sealed class DeletePlanHandler : IRequestHandler<DeletePlanCommand, Result>
{
	// DAOs
	private readonly IPlanRepository _planRepository;

	// UNIT OF WORK
	private readonly IUnitOfWork _unitOfWork;

	public DeletePlanHandler(
		IPlanRepository planRepository, 
		IUnitOfWork unitOfWork
		)
	{
		_planRepository = planRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(DeletePlanCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync( async () =>
		{
			var plan = await _planRepository.GetByIdAsync(
				PlanId.From(request.PlanId),
				cancellationToken);

			if (plan is null)
			{
				return Result.Failure(PlanErrors.NotFound);
			}

			if (plan.Status != PlanStatus.Draft) // if not draft then continue to check
			{
				// Prevent deletion if plan has active subscriptions
				if (plan.Subscriptions.Any() == true)
				{
					return Result.Failure(PlanErrors.CannotDeletePlanWithSubscriptions);
				}
			}

			_planRepository.Delete(plan);

			return Result.Success();
		}, cancellationToken);
	}
}
