using MediatR;
using Rakushu.Application.Usecases.Transaction.ExpireTransactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Worker.Workers.Payment;

public sealed class TransactionExpiryWorker(IServiceScopeFactory scopeFactory, ILogger<TransactionExpiryWorker> logger)
	: BackgroundService
{
	private static readonly TimeSpan Interval = TimeSpan.FromSeconds(5);

	protected override async Task ExecuteAsync(
		CancellationToken stoppingToken)
	{
		logger.LogInformation("Transaction expiration worker started.");

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await using var scope = scopeFactory.CreateAsyncScope();

				var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

				var result = await mediator.Send(new ExpireTransactionsCommand(), stoppingToken);

				if (result.IsFailure)
				{
					logger.LogError("Failed to expire transactions: {Error}", result.Error);
				}
			}
			catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
			{
				break;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error while expiring transactions.");
			}

			await Task.Delay(Interval, stoppingToken);
		}

		logger.LogInformation("Transaction expiration worker stopped.");
	}
}