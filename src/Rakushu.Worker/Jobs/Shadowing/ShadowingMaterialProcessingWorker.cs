using MediatR;
using Rakushu.Application.Usecases.Shadowing.ShadowingMaterials.GenerateShadowingMaterials;

namespace Rakushu.Worker.Jobs.Shadowing;

public sealed class ShadowingMaterialProcessingWorker(
	IServiceScopeFactory scopeFactory,
	ILogger<ShadowingMaterialProcessingWorker> logger) : BackgroundService
{
	private const int BatchSize = 3;
	private static readonly TimeSpan Interval = TimeSpan.FromSeconds(5);

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		logger.LogInformation("Shadowing material processing worker started.");

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await using var scope = scopeFactory.CreateAsyncScope();
				var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

				var result = await mediator.Send(new GenerateShadowingMaterialsCommand(BatchSize), stoppingToken);

				if (result.IsFailure)
					logger.LogError("Shadowing material processing failed: {Error}", result.Error);
				else if (result.Value > 0)
					logger.LogInformation("Moved {Count} shadowing material(s) to Ready.", result.Value);
			}
			catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
			{
				break;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Unexpected error while processing shadowing materials.");
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

		logger.LogInformation("Shadowing material processing worker stopped.");
	}
}
