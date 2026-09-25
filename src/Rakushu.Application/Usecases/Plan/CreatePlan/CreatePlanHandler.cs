using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Payment;
using Rakushu.Domain.Entities.Plan;
using Rakushu.Domain.Entities.Plan.ObjectValues;

namespace Rakushu.Application.Usecases.Plan.CreatePlan;

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
			var planCodeResult = PlanCode.Create(request.Code);

			if (planCodeResult.IsFailure)
			{
				return Result.Failure<Guid>(planCodeResult.Error);
			}

			var existingPlan = await _planRepository.GetByCodeAsync(planCodeResult.Value, cancellationToken);

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

			var now = _systemClock.UtcNow;

			var initialStatus = PlanStatus.Draft;

			// 3. Create plan
			var planResult = Domain.Entities.Plan.Plan.Create(
				planCodeResult.Value,
				request.Name,
				request.Price,
				currency,
				billingCycle,
				initialStatus,
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
