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

namespace Rakushu.Application.Usecases.Subscription.Subscription.ExpireSubscription;


internal sealed class ExpireSubscriptionsHandler : IRequestHandler<ExpireSubscriptionsCommand, Result>
{
	// DAOS
	private readonly ISubscriptionRepository _subscriptionRepository;

	// UNIT OF WORK
	private readonly IUnitOfWork _unitOfWork;

	// SERVICES
	private readonly ISystemClock _systemClock;

	public ExpireSubscriptionsHandler(
		ISubscriptionRepository subscriptionRepository,
		IUnitOfWork unitOfWork,
		ISystemClock systemClock)
	{
		_subscriptionRepository = subscriptionRepository;
		_unitOfWork = unitOfWork;
		_systemClock = systemClock;
	}

	public async Task<Result> Handle(
		ExpireSubscriptionsCommand request,
		CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var now = _systemClock.UtcNow;

			var subscriptions = (await _subscriptionRepository.GetAllAsync(cancellationToken))
									.Where(s => s.Status == SubscriptionStatus.Active);

			var expiredSubscriptions = subscriptions.Where(s => s.EndAt <= now).ToList();

			foreach (var subscription in expiredSubscriptions)
			{
				var result = subscription.Expire(now);

				if (result.IsFailure)
				{
					return result;
				}
			}

			return Result.Success();

		}, cancellationToken);
	}
}