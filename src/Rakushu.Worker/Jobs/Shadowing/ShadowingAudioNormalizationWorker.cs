using MediatR;
using Rakushu.Application.Usecases.Shadowing.ShadowingAssessments;

namespace Rakushu.Worker.Jobs.Shadowing;

public sealed class ShadowingAudioNormalizationWorker(
	IServiceScopeFactory scopeFactory,
	IConfiguration configuration,
	ILogger<ShadowingAudioNormalizationWorker> logger) : BackgroundService
{
	private static readonly TimeSpan Interval = TimeSpan.FromSeconds(10);

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var batchSize = configuration.GetValue("ShadowingNormalization:BatchSize", 5);
		logger.LogInformation("Shadowing audio normalization worker started.");

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await using var scope = scopeFactory.CreateAsyncScope();
				var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
				var result = await mediator.Send(new NormalizeCompletedShadowingAttemptsCommand(batchSize), stoppingToken);

				if (result.IsFailure)
					logger.LogError("Shadowing audio normalization failed: {Error}", result.Error);
				else if (result.Value > 0)
					logger.LogInformation("Normalized {Count} shadowing attempt(s).", result.Value);
			}
			catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
			{
				break;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Unexpected error while normalizing shadowing attempts.");
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

		logger.LogInformation("Shadowing audio normalization worker stopped.");
	}
}
