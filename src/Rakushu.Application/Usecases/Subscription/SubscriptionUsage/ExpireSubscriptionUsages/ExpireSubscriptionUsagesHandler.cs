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

namespace Rakushu.Application.Usecases.Subscription.SubscriptionUsage.ExpireSubscriptionUsages;

internal sealed class ExpireSubscriptionUsagesHandler : IRequestHandler<ExpireSubscriptionUsagesCommand, Result>
{
	// DAOS
	private readonly ISubscriptionRepository _subscriptionRepository;

	// UNIT OF WORK
	private readonly IUnitOfWork _unitOfWork;

	// SERVICES
	private readonly ISystemClock _systemClock;

	public ExpireSubscriptionUsagesHandler(
		ISubscriptionRepository subscriptionRepository,
		IUnitOfWork unitOfWork,
		ISystemClock systemClock)
	{
		_subscriptionRepository = subscriptionRepository;
		_unitOfWork = unitOfWork;
		_systemClock = systemClock;
	}

	public async Task<Result> Handle(
		ExpireSubscriptionUsagesCommand request,
		CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var now = _systemClock.UtcNow;

			var expiredUsages = (await _subscriptionRepository.GetAllAsync(cancellationToken))
									.Where(s => s.Status == SubscriptionStatus.Active)
									.SelectMany(s => s.SubscriptionUsages
										.Where(u => u.IsExpired == false && u.PeriodEnd <= now)).ToList();

			//var expiredUsages = usages
			//	.Where(u => u.IsExpired == false && u.PeriodEnd <= now)
			//	.ToList();

			foreach (var usage in expiredUsages)
			{
				var result = usage.Expire(now);

				if (result.IsFailure)
				{
					return result;
				}
			}

			return Result.Success();
		}, cancellationToken);
	}
}