using MediatR;
using Rakushu.Application.Usecases.Shadowing.ShadowingAttempts;

namespace Rakushu.Worker.Jobs.Shadowing;

public sealed class ShadowingSessionCompletionWorker(
	IServiceScopeFactory scopeFactory,
	ILogger<ShadowingSessionCompletionWorker> logger) : BackgroundService
{
	private const int BatchSize = 100;
	private static readonly TimeSpan Interval = TimeSpan.FromSeconds(30);

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		logger.LogInformation("Shadowing session completion worker started.");

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await using var scope = scopeFactory.CreateAsyncScope();
				var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

				var result = await mediator.Send(new CompleteFinishedShadowingSessionsCommand(BatchSize), stoppingToken);

				if (result.IsFailure)
					logger.LogError("Session completion failed: {Error}", result.Error);
				else if (result.Value > 0)
					logger.LogInformation("Finished {Count} shadowing session(s).", result.Value);
			}
			catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
			{
				break;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Unexpected error while completing shadowing sessions.");
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

		logger.LogInformation("Shadowing session completion worker stopped.");
	}
}
