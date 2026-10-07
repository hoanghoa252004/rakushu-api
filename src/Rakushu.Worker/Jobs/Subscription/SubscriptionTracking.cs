using MediatR;
using Rakushu.Application.Usecases.Subscription.Subscription.ExpireSubscription;
using Rakushu.Application.Usecases.Subscription.SubscriptionUsage.CreateSubscriptionUsages;
using Rakushu.Application.Usecases.Subscription.SubscriptionUsage.ExpireSubscriptionUsages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Worker.Jobs.Subscription;

public sealed class SubscriptionTracking(IServiceScopeFactory scopeFactory, ILogger<SubscriptionTracking> logger)
	: BackgroundService
{
	private static readonly TimeSpan Interval = TimeSpan.FromSeconds(10);

	protected override async Task ExecuteAsync(
		CancellationToken stoppingToken)
	{
		logger.LogInformation("Subscription tracking worker started.");

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await using var scope = scopeFactory.CreateAsyncScope();

				var mediator = scope.ServiceProvider
					.GetRequiredService<IMediator>();

				// 1. Create usages for new periods
				var createResult = await mediator.Send(
					new CreateSubscriptionUsagesCommand(),
					stoppingToken);

				if (createResult.IsFailure)
				{
					logger.LogError(
						"Failed to create subscription usages: {Error}",
						createResult.Error);
				}

				// 2. Expire old usages
				var expireUsageResult = await mediator.Send(
					new ExpireSubscriptionUsagesCommand(),
					stoppingToken);

				if (expireUsageResult.IsFailure)
				{
					logger.LogError(
						"Failed to expire subscription usages: {Error}",
						expireUsageResult.Error);
				}

				// 3. Expire subscriptions
				var expireSubscriptionResult = await mediator.Send(
					new ExpireSubscriptionsCommand(),
					stoppingToken);

				if (expireSubscriptionResult.IsFailure)
				{
					logger.LogError(
						"Failed to expire subscriptions: {Error}",
						expireSubscriptionResult.Error);
				}
			}
			catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
			{
				break;
			}
			catch (Exception ex)
			{
				logger.LogError(
					ex,
					"Error while processing subscriptions.");
			}

			await Task.Delay(Interval, stoppingToken);
		}

		logger.LogInformation("Subscription tracking worker stopped.");
	}
}