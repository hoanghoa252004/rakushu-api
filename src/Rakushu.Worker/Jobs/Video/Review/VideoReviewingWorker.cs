using MediatR;
using Rakushu.Application.Usecases.Material.Video.PromoteProcessedVideos;

namespace Rakushu.Worker.Jobs.Video.Review;

public sealed class VideoReviewingWorker(
	IServiceScopeFactory scopeFactory,
	ILogger<VideoReviewingWorker> logger) : BackgroundService
{
	private const int BatchSize = 10;
	private static readonly TimeSpan Interval = TimeSpan.FromSeconds(30);

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		logger.LogInformation("Video reviewing worker started.");

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await using var scope = scopeFactory.CreateAsyncScope();
				var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

				var result = await mediator.Send(new PromoteProcessedVideosCommand(BatchSize), stoppingToken);

				if (result.IsFailure)
					logger.LogError("Video reviewing check failed: {Error}", result.Error);
				else if (result.Value > 0)
					logger.LogInformation("Moved {Count} video(s) to Reviewing.", result.Value);
			}
			catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
			{
				break;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Unexpected error while promoting processed videos.");
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

		logger.LogInformation("Video reviewing worker stopped.");
	}
}
