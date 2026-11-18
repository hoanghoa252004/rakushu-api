using MediatR;
using Rakushu.Application.Usecases.Material.Video.ModerateVideos;

namespace Rakushu.Worker.Jobs.Video.Moderation;

public sealed class VideoModerationWorker(
	IServiceScopeFactory scopeFactory,
	ILogger<VideoModerationWorker> logger) : BackgroundService
{
	private const int BatchSize = 10;
	private static readonly TimeSpan Interval = TimeSpan.FromSeconds(10);

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		logger.LogInformation("Video moderation worker started.");

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await using var scope = scopeFactory.CreateAsyncScope();
				var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

				var result = await mediator.Send(new ModerateVideosCommand(BatchSize), stoppingToken);

				if (result.IsFailure)
					logger.LogError("Video moderation failed: {Error}", result.Error);
				else if (result.Value > 0)
					logger.LogInformation("Moderated {Count} video(s).", result.Value);
			}
			catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
			{
				break;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Unexpected error while moderating videos.");
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
	}
}
