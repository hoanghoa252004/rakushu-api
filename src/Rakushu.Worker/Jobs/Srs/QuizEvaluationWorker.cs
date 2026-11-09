using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.EvaluatePendingQuizAttempts;

namespace Rakushu.Worker.Jobs.Srs;

public sealed class QuizEvaluationWorker(
	IServiceScopeFactory scopeFactory,
	ILogger<QuizEvaluationWorker> logger) : BackgroundService
{
	private const int BatchSize = 10;
	private static readonly TimeSpan Interval = TimeSpan.FromSeconds(3);

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		logger.LogInformation("SRS Quiz evaluation worker started.");

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await using var scope = scopeFactory.CreateAsyncScope();
				var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

				var result = await mediator.Send(new EvaluatePendingQuizAttemptsCommand(BatchSize), stoppingToken);

				if (result.IsFailure)
				{
					logger.LogError("SRS Quiz evaluation check failed: {Error}", result.Error);
				}
				else if (result.Value > 0)
				{
					logger.LogInformation("Successfully evaluated {Count} quiz attempt(s).", result.Value);
				}
			}
			catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
			{
				break;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Unexpected error in SRS QuizEvaluationWorker.");
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

		logger.LogInformation("SRS Quiz evaluation worker stopped.");
	}
}
