using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.ExpireStaleQuizAttempts;

namespace Rakushu.Worker.Jobs.Srs;

public sealed class StaleQuizAttemptCleanupWorker(
	IServiceScopeFactory scopeFactory,
	ILogger<StaleQuizAttemptCleanupWorker> logger) : BackgroundService
{
	private const int BatchSize = 50;
	private static readonly TimeSpan Interval = TimeSpan.FromMinutes(1);

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		logger.LogInformation("SRS Stale Quiz Attempt cleanup worker started.");

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await using var scope = scopeFactory.CreateAsyncScope();
				var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

				var result = await mediator.Send(new ExpireStaleQuizAttemptsCommand(BatchSize), stoppingToken);

				if (result.IsFailure)
				{
					logger.LogError("Stale quiz attempt cleanup failed: {Error}", result.Error);
				}
				else if (result.Value > 0)
				{
					logger.LogInformation("Cleaned up and expired {Count} stale quiz attempt(s).", result.Value);
				}
			}
			catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
			{
				break;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Unexpected error in StaleQuizAttemptCleanupWorker.");
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

		logger.LogInformation("SRS Stale Quiz Attempt cleanup worker stopped.");
	}
}
