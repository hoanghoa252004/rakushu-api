using MediatR;
using Rakushu.Application.Usecases.Shadowing.ShadowingAssessments;

namespace Rakushu.Worker.Jobs.Shadowing;

public sealed class ShadowingAssessmentWorker(
	IServiceScopeFactory scopeFactory,
	IConfiguration configuration,
	ILogger<ShadowingAssessmentWorker> logger) : BackgroundService
{
	private static readonly TimeSpan Interval = TimeSpan.FromSeconds(30);

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var batchSize = configuration.GetValue("ShadowingAssessment:BatchSize", 5);
		logger.LogInformation("Shadowing assessment worker started.");

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await using var scope = scopeFactory.CreateAsyncScope();
				var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
				var result = await mediator.Send(new AssessCompletedShadowingAttemptsCommand(batchSize), stoppingToken);

				if (result.IsFailure)
					logger.LogError("Shadowing assessment failed: {Error}", result.Error);
				else if (result.Value > 0)
					logger.LogInformation("Assessed {Count} shadowing attempt(s).", result.Value);
			}
			catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
			{
				break;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Unexpected error while assessing shadowing attempts.");
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

		logger.LogInformation("Shadowing assessment worker stopped.");
	}
}
