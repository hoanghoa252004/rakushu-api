using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rakushu.Application.Usecases.Payment.ExpirePendingPayments;

namespace Rakushu.Infrastructure.Payment;

public sealed class PaymentExpirationBackgroundService : BackgroundService
{
	private readonly IServiceProvider _serviceProvider;
	private readonly ILogger<PaymentExpirationBackgroundService> _logger;

	public PaymentExpirationBackgroundService(
		IServiceProvider serviceProvider,
		ILogger<PaymentExpirationBackgroundService> logger)
	{
		_serviceProvider = serviceProvider;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

		while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
		{
			try
			{
				using var scope = _serviceProvider.CreateScope();
				var sender = scope.ServiceProvider.GetRequiredService<ISender>();
				var result = await sender.Send(new ExpirePendingPaymentsCommand(), stoppingToken);

				if (result.IsSuccess && result.Value > 0)
				{
					_logger.LogInformation("Expired {Count} pending payment(s).", result.Value);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while expiring pending payments.");
			}
		}
	}
}
