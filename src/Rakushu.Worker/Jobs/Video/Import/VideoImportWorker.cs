using MediatR;
using Rakushu.Application.Usecases.Material.Video.ImportExternalVideos;

namespace Rakushu.Worker.Jobs.Video.Import;

public sealed class VideoImportWorker(
	IServiceScopeFactory scopeFactory,
	ILogger<VideoImportWorker> logger) : BackgroundService
{
	private const int BatchSize = 1;
	private static readonly TimeSpan Interval = TimeSpan.FromSeconds(1);

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		logger.LogInformation("Video import worker started.");

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await using var scope = scopeFactory.CreateAsyncScope();
				var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

				var result = await mediator.Send(new ImportExternalVideosCommand(BatchSize), stoppingToken);

				if (result.IsFailure)
					logger.LogError("Video import failed: {Error}", result.Error);
				else if (result.Value > 0)
					logger.LogInformation("Imported {Count} video(s) from external source.", result.Value);
			}
			catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
			{
				break;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Unexpected error while importing external videos.");
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

		logger.LogInformation("Video import worker stopped.");
	}
}
