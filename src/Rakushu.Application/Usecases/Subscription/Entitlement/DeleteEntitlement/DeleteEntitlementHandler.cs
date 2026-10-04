using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Plan;
using Rakushu.Domain.Entities.Plan.Entitlement;

namespace Rakushu.Application.Usecases.Subscription.Entitlement.DeleteEntitlement;

internal sealed class DeleteEntitlementHandler : IRequestHandler<DeleteEntitlementCommand, Result>
{
	private readonly IPlanRepository _planRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ISystemClock _systemClock;

	public DeleteEntitlementHandler(
		IPlanRepository planRepository,
		IUnitOfWork unitOfWork,
		ISystemClock systemClock)
	{
		_planRepository = planRepository;
		_unitOfWork = unitOfWork;
		_systemClock = systemClock;
	}

	public async Task<Result> Handle(DeleteEntitlementCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var plan = await _planRepository.GetByIdAsync(
				PlanId.From(request.PlanId),
				cancellationToken);

			if (plan is null)
			{
				return Result.Failure(PlanErrors.NotFound);
			}

			var now = _systemClock.UtcNow;

			var result = plan.RemoveEntitlement(
				EntitlementId.From(request.EntitlementId),
				now);

			if (result.IsFailure)
			{
				return result;
			}

			return Result.Success();
		}, cancellationToken);
	}
}
