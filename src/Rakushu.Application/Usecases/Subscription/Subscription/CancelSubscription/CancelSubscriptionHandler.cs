using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User.Subscription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Subscription.Subscription.CancelSubscription;

internal sealed class CancelSubscriptionHandler : IRequestHandler<CancelSubscriptionCommand, Result>
{
	// DAOS
	private readonly ISubscriptionRepository _subscriptionRepository;

	// UNIT OF WORK
	private readonly IUnitOfWork _unitOfWork;

	// SERVICES
	private readonly ISystemClock _systemClock;

	public CancelSubscriptionHandler(
		ISubscriptionRepository subscriptionRepository,
		IUnitOfWork unitOfWork,
		ISystemClock systemClock)
	{
		_subscriptionRepository = subscriptionRepository;
		_unitOfWork = unitOfWork;
		_systemClock = systemClock;
	}

	public async Task<Result> Handle(
		CancelSubscriptionCommand request,
		CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var now = _systemClock.UtcNow;

			var subcriptionId = SubscriptionId.From(request.Id);

			var subscription = await _subscriptionRepository.GetByIdAsync(subcriptionId, cancellationToken);

			if(subscription is null)
			{
				return Result.Failure(SubscriptionErrors.NotFound);
			}

			if(subscription.Status != SubscriptionStatus.Active)
				return Result.Failure(SubscriptionErrors.InvalidStatus);

			var subscriptionResult = subscription.Cancel(now);

			if (subscriptionResult.IsFailure)
			{
				return subscriptionResult;
			}

			foreach (var usage in subscription.SubscriptionUsages.Where(u => u.IsExpired == false && u.IsCanceled == false))
			{
				var usageResult = usage.Cancel(now);

				if (usageResult.IsFailure)
				{
					return usageResult;
				}
			}

			return Result.Success();

		}, cancellationToken);
	}
}