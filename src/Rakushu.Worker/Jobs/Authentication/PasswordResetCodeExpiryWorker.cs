using MediatR;
using Rakushu.Application.Usecases.Authentication.ExpirePasswordResetCodes;

namespace Rakushu.Worker.Jobs.Authentication;

public sealed class PasswordResetCodeExpiryWorker(IServiceScopeFactory scopeFactory, ILogger<PasswordResetCodeExpiryWorker> logger)
	: BackgroundService
{
	private static readonly TimeSpan Interval = TimeSpan.FromSeconds(5);

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		logger.LogInformation("Password reset code expiration worker started.");

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await using var scope = scopeFactory.CreateAsyncScope();

				var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

				var result = await mediator.Send(new ExpirePasswordResetCodesCommand(), stoppingToken);

				if (result.IsFailure)
				{
					logger.LogError("Failed to expire password reset codes: {Error}", result.Error);
				}
			}
			catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
			{
				break;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error while expiring password reset codes.");
			}

			await Task.Delay(Interval, stoppingToken);
		}

		logger.LogInformation("Password reset code expiration worker stopped.");
	}
}
