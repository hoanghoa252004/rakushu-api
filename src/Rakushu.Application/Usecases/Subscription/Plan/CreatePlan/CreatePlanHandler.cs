using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Payment;
using Rakushu.Domain.Entities.Plan;

namespace Rakushu.Application.Usecases.Subscription.Plan.CreatePlan;

public sealed class CreatePlanHandler : IRequestHandler<CreatePlanCommand, Result<Guid>>
{
	// DAOs
	private readonly IPlanRepository _planRepository;
	
	// UNIT OF WORK
	private readonly IUnitOfWork _unitOfWork;

	// SERVICES
	private readonly ISystemClock _systemClock;

	public CreatePlanHandler(
		IPlanRepository planRepository, 
		IUnitOfWork unitOfWork,
		ISystemClock systemClock
		)
	{
		_planRepository = planRepository;
		_unitOfWork = unitOfWork;
		_systemClock = systemClock;
	}

	public async Task<Result<Guid>> Handle(CreatePlanCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			// 1. Create PlanCode
			var existingPlan = await _planRepository.GetByCodeAsync(request.Code, cancellationToken);

			if (existingPlan != null) // Check if code already exists
			{
				return Result.Failure<Guid>(PlanErrors.DuplicateCode(request.Code));
			}

			// 2. Parse enums
			if (!Enum.TryParse<Currency>(request.Currency, true, out var currency))
			{
				return Result.Failure<Guid>(PlanErrors.InvalidCurrency);
			}

			if (!Enum.TryParse<BillingCycle>(request.BillingCycle, true, out var billingCycle))
			{
				return Result.Failure<Guid>(PlanErrors.InvalidBillingCycle);
			}

			// 3. Check any plan with price = 0 ( only allow 1 free plan )
			if(request.Price == 0)
			{
				var freePlan = (await _planRepository.GetAllAsync(cancellationToken))
					.SingleOrDefault(p => p.Price == 0);
				if (freePlan is not null)
				{
					return Result.Failure<Guid>(PlanErrors.DuplicateFreePlan);
				}
			}

			var now = _systemClock.UtcNow;

			// 3. Create plan
			var planResult = Domain.Entities.Plan.Plan.Create(
				request.Code,
				request.Name,
				request.JapaneseName,
				request.Price,
				currency,
				billingCycle,
				request.IsActive,
				now,
				now,
				request.Description
			);

			if (planResult.IsFailure)
			{
				return Result.Failure<Guid>(planResult.Error);
			}

			_planRepository.Add(planResult.Value);

			return Result.Success(planResult.Value.Id.Value);
		}, cancellationToken);
	}
}
