using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rakushu.Application.Usecases.Learning.Srs.BankQuestion.GenerateDraftQuestions;

namespace Rakushu.Worker.Jobs.Srs;

public sealed class DraftQuestionGenerationWorker(
	IServiceScopeFactory scopeFactory,
	ILogger<DraftQuestionGenerationWorker> logger) : BackgroundService
{
	private const int BatchSize = 10;
	private static readonly TimeSpan Interval = TimeSpan.FromSeconds(5);

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		logger.LogInformation("SRS Draft question generation worker started.");

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await using var scope = scopeFactory.CreateAsyncScope();
				var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

				var result = await mediator.Send(new GenerateDraftQuestionsCommand(BatchSize), stoppingToken);

				if (result.IsFailure)
				{
					logger.LogError("Draft question generation failed: {Error}", result.Error);
				}
				else if (result.Value > 0)
				{
					logger.LogInformation("Generated {Count} draft quiz question(s) waiting for curator review.", result.Value);
				}
			}
			catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
			{
				break;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Unexpected error in DraftQuestionGenerationWorker.");
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

		logger.LogInformation("SRS Draft question generation worker stopped.");
	}
}
