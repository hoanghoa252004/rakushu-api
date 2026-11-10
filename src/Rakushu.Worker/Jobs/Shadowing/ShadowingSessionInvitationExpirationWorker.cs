using MediatR;
using Rakushu.Application.Usecases.Shadowing.ShadowingSessionInvitations;

namespace Rakushu.Worker.Jobs.Shadowing;

public sealed class ShadowingSessionInvitationExpirationWorker(
	IServiceScopeFactory scopeFactory,
	ILogger<ShadowingSessionInvitationExpirationWorker> logger) : BackgroundService
{
	private const int BatchSize = 100;
	private static readonly TimeSpan Interval = TimeSpan.FromSeconds(30);

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		logger.LogInformation("Shadowing session invitation expiration worker started.");

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await using var scope = scopeFactory.CreateAsyncScope();
				var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

				var result = await mediator.Send(new ExpireShadowingSessionInvitationsCommand(BatchSize), stoppingToken);

				if (result.IsFailure)
					logger.LogError("Invitation expiration failed: {Error}", result.Error);
				else if (result.Value > 0)
					logger.LogInformation("Expired {Count} session invitation(s).", result.Value);
			}
			catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
			{
				break;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Unexpected error while expiring session invitations.");
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

		logger.LogInformation("Shadowing session invitation expiration worker stopped.");
	}
}
