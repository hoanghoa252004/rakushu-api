using MediatR;
using Rakushu.Application.Usecases.Shadowing.ShadowingSessionInvitations;

namespace Rakushu.Worker.Jobs.Shadowing;

public sealed class ShadowingSessionExpirationWorker(
	IServiceScopeFactory scopeFactory,
	ILogger<ShadowingSessionExpirationWorker> logger) : BackgroundService
{
	private const int BatchSize = 100;
	private static readonly TimeSpan Interval = TimeSpan.FromSeconds(30);

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		logger.LogInformation("Shadowing session expiration worker started.");

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await using var scope = scopeFactory.CreateAsyncScope();
				var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

				var result = await mediator.Send(new ExpireShadowingSessionsCommand(BatchSize), stoppingToken);

				if (result.IsFailure)
					logger.LogError("Session expiration failed: {Error}", result.Error);
				else if (result.Value > 0)
					logger.LogInformation("Expired {Count} shadowing session(s) and their pending invitations.", result.Value);
			}
			catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
			{
				break;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Unexpected error while expiring shadowing sessions.");
			}

			try
			{
				await Task.Delay(Interval, stoppingToken);
			}
			catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
			{
				break;
			}
		}

		logger.LogInformation("Shadowing session expiration worker stopped.");
	}
}
