using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Plan.Entitlement;
using Rakushu.Domain.Entities.User.Subscription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Subscription.SubscriptionUsage.CreateSubscriptionUsages;

internal sealed class CreateSubscriptionUsagesHandler
	: IRequestHandler<CreateSubscriptionUsagesCommand, Result>
{
	// DAOS
	private readonly ISubscriptionRepository _subscriptionRepository;

	// UNIT OF WORK
	private readonly IUnitOfWork _unitOfWork;

	// SERVICES
	private readonly ISystemClock _systemClock;

	public CreateSubscriptionUsagesHandler(
		ISubscriptionRepository subscriptionRepository,
		IUnitOfWork unitOfWork,
		ISystemClock systemClock)
	{
		_subscriptionRepository = subscriptionRepository;
		_unitOfWork = unitOfWork;
		_systemClock = systemClock;
	}

	public async Task<Result> Handle(
		CreateSubscriptionUsagesCommand request,
		CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var now = _systemClock.UtcNow;

			var subscriptions = (await _subscriptionRepository.GetAllAsync(cancellationToken))
									.Where(s => s.Status == SubscriptionStatus.Active);

			foreach (var subscription in subscriptions)
			{
				var entitlements = subscription.Plan.Entitlements;

				foreach (var entitlement in entitlements)
				{
					if (entitlement.IsEnabled == false)
						continue;

					var period = CalculatePeriod(subscription, entitlement, now);

					if (period is null)
						continue;

					var exists = subscription.SubscriptionUsages
						.Any(u => u.SubscriptionId == subscription.Id
							&& u.FeatureId == entitlement.FeatureId
							&& u.PeriodStart == period.Value.Start
							&& u.PeriodEnd == period.Value.End
							&& u.IsExpired == false);

					if (exists)
						continue;

					var usageResult = subscription.AddUsage(
						entitlement.FeatureId,
						period.Value.Start,
						period.Value.End,
						entitlement.LimitValue,
						0,
						false,
						false,
						false,
						now);

					if (usageResult.IsFailure)
						return usageResult;
				}
			}

			return Result.Success();
		}, cancellationToken);
	}

	private static (DateTimeOffset Start, DateTimeOffset End)? CalculatePeriod(
		Domain.Entities.User.Subscription.Subscription subscription,
		Domain.Entities.Plan.Entitlement.Entitlement entitlement,
		DateTimeOffset now)
	{
		return entitlement.LimitPeriod switch
		{
			LimitPeriod.Day => CalculateDailyPeriod(now),

			LimitPeriod.Total => (subscription.StartAt, subscription.EndAt),

			_ => null
		};
	}

	private static (DateTimeOffset Start, DateTimeOffset End) CalculateDailyPeriod(DateTimeOffset now)
	{
		var start = new DateTimeOffset(
			now.Year,
			now.Month,
			now.Day,
			0,
			0,
			0,
			now.Offset);

		var end = start.AddDays(1);

		return (start, end);
	}
}