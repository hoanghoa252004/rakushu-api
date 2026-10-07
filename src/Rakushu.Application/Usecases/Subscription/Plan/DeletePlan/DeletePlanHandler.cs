using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Plan;

namespace Rakushu.Application.Usecases.Subscription.Plan.DeletePlan;

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

			// Prevent deletion if plan has active subscriptions
			if (plan.Subscriptions.Any() == true)
			{
				return Result.Failure(PlanErrors.CannotDeletePlanWithSubscriptions);
			}

			_planRepository.Delete(plan);

			return Result.Success();
		}, cancellationToken);
	}
}
