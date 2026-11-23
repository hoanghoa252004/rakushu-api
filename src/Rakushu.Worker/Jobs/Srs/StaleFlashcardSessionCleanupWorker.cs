using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rakushu.Application.Usecases.Learning.Srs.Flashcards.ExpireStaleFlashcardSessions;

namespace Rakushu.Worker.Jobs.Srs;

public sealed class StaleFlashcardSessionCleanupWorker(
	IServiceScopeFactory scopeFactory,
	ILogger<StaleFlashcardSessionCleanupWorker> logger) : BackgroundService
{
	private const int BatchSize = 50;
	private static readonly TimeSpan Interval = TimeSpan.FromMinutes(1);

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		logger.LogInformation("SRS Stale Flashcard Session cleanup worker started.");

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await using var scope = scopeFactory.CreateAsyncScope();
				var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

				var result = await mediator.Send(new ExpireStaleFlashcardSessionsCommand(BatchSize), stoppingToken);

				if (result.IsFailure)
				{
					logger.LogError("Stale flashcard session cleanup failed: {Error}", result.Error);
				}
				else if (result.Value > 0)
				{
					logger.LogInformation("Cleaned up and expired {Count} stale flashcard session(s).", result.Value);
				}
			}
			catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
			{
				break;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Unexpected error in StaleFlashcardSessionCleanupWorker.");
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

		logger.LogInformation("SRS Stale Flashcard Session cleanup worker stopped.");
	}
}
