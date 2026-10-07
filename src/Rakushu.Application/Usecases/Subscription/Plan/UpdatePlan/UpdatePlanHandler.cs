using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Payment;
using Rakushu.Domain.Entities.Plan;

namespace Rakushu.Application.Usecases.Subscription.Plan.UpdatePlan;

public sealed class UpdatePlanHandler : IRequestHandler<UpdatePlanCommand, Result>
{
	// DAOs
	private readonly IPlanRepository _planRepository;

	// UNIT OF WORK
	private readonly IUnitOfWork _unitOfWork;

	// SERVICES
	private readonly ISystemClock _systemClock;

	public UpdatePlanHandler(
		IPlanRepository planRepository,
		IUnitOfWork unitOfWork,
		ISystemClock systemClock
		)
	{
		_planRepository = planRepository;
		_unitOfWork = unitOfWork;
		_systemClock = systemClock;
	}

	public async Task<Result> Handle(UpdatePlanCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync( async () =>
		{
			// 1. Get plan
			var plan = await _planRepository.GetByIdAsync(
				PlanId.From(request.PlanId),
				cancellationToken);

			if (plan == null)
			{
				return Result.Failure(PlanErrors.NotFound);
			}

			// 2. Parse enum
			if (!Enum.TryParse<Currency>(request.Currency, true, out var currency))
			{
				return Result.Failure<Guid>(PlanErrors.InvalidCurrency);
			}

			if (!Enum.TryParse<BillingCycle>(request.BillingCycle, true, out var billingCycle))
			{
				return Result.Failure<Guid>(PlanErrors.InvalidBillingCycle);
			}

			// 3. Check any plan with price = 0 ( only allow 1 free plan )
			if (request.Price == 0)
			{
				var freePlan = (await _planRepository.GetAllAsync(cancellationToken))
					.SingleOrDefault(p => p.Price == 0);
				if (freePlan is not null)
				{
					return Result.Failure<Guid>(PlanErrors.DuplicateFreePlan);
				}
			}

			var now = _systemClock.UtcNow;

			// 4. Update domain
			var result = plan.Update(
				request.Name,
				request.JapaneseName,
				request.Price,
				currency,
				billingCycle,
				request.IsActive,
				now,
				request.Description);

			if (result.IsFailure)
			{
				return result;
			}

			return Result.Success();
		}, cancellationToken);
	}
}
