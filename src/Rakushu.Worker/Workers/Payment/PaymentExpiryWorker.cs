using MediatR;
using Rakushu.Application.Usecases.Payment.ExpirePayments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Worker.Workers.Payment;

public sealed class PaymentExpirationWorker(IServiceScopeFactory scopeFactory, ILogger<PaymentExpirationWorker> logger) 
	: BackgroundService
{
	private static readonly TimeSpan Interval = TimeSpan.FromSeconds(5);

	protected override async Task ExecuteAsync(
		CancellationToken stoppingToken)
	{
		logger.LogInformation("Payment expiration worker started.");

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await using var scope =scopeFactory.CreateAsyncScope();

				var mediator =scope.ServiceProvider.GetRequiredService<IMediator>();

				var result = await mediator.Send(new ExpirePaymentsCommand(), stoppingToken);

				if(result.IsFailure)
				{
					logger.LogError("Failed to expire payments: {Error}", result.Error);
				}
			}
			catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
			{
				break;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error while expiring payments.");
			}

			await Task.Delay(Interval, stoppingToken);
		}

		logger.LogInformation("Payment expiration worker stopped.");
	}
}