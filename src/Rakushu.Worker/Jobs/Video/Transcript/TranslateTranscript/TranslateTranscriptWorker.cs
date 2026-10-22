using MediatR;
using Rakushu.Application.Usecases.Material.Transcript.ImportTranscript;
using Rakushu.Application.Usecases.Material.Transcript.TranslateTranscript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Worker.Jobs.Video.Transcript.TranslateTranscript;

public sealed class TranslateTranscriptWorker(IServiceScopeFactory scopeFactory, ILogger<TranslateTranscriptWorker> logger) : BackgroundService
{
	private static readonly TimeSpan Interval =
		TimeSpan.FromSeconds(5);

	protected override async Task ExecuteAsync(
		CancellationToken stoppingToken)
	{
		logger.LogInformation("TranslateTranscriptWorker started.");

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await using var scope =
					scopeFactory.CreateAsyncScope();

				var mediator = scope.ServiceProvider
					.GetRequiredService<IMediator>();

				var result = await mediator.Send(
					new TranslateTranscriptCommand(),
					stoppingToken);

				if (result.IsFailure)
				{
					logger.LogError(
						"TranslateTranscriptWorker failed: {Error}",
						result.Error);
				}
			}
			catch (OperationCanceledException)
				when (stoppingToken.IsCancellationRequested)
			{
				break;
			}
			catch (Exception ex)
			{
				logger.LogError(
					ex,
					"Unexpected error while TranslateTranscriptWorker.");
			}

			try
			{
				await Task.Delay(Interval, stoppingToken);
			}
			catch (OperationCanceledException)
				when (stoppingToken.IsCancellationRequested)
			{
				break;
			}
		}

		logger.LogInformation("TranslateTranscriptWorker stopped.");
	}
}