using MediatR;
using Rakushu.Application.Usecases.Material.Transcript.ImportTranscript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Worker.Jobs.Video.Transcript.ImportTranscript;

public sealed class ImportTranscriptWorker(IServiceScopeFactory scopeFactory, ILogger<ImportTranscriptWorker> logger) : BackgroundService
{
	private static readonly TimeSpan Interval =
		TimeSpan.FromSeconds(10);

	protected override async Task ExecuteAsync(
		CancellationToken stoppingToken)
	{
		logger.LogInformation("Transcript import worker started.");

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await using var scope =
					scopeFactory.CreateAsyncScope();

				var mediator = scope.ServiceProvider
					.GetRequiredService<IMediator>();

				var result = await mediator.Send(
					new ImportTranscriptCommand(),
					stoppingToken);

				if (result.IsFailure)
				{
					logger.LogError(
						"Transcript import failed: {Error}",
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
					"Unexpected error while importing transcripts.");
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

		logger.LogInformation("Transcript import worker stopped.");
	}
}