using MediatR;
using Rakushu.Application.Usecases.Material.Nlp.NlpProcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Worker.Jobs.Video.Nlp;

public sealed class NlpProcessWorker(
	IServiceScopeFactory scopeFactory,
	ILogger<NlpProcessWorker> logger) : BackgroundService
{
	private static readonly TimeSpan Interval =
		TimeSpan.FromSeconds(10);

	protected override async Task ExecuteAsync(
		CancellationToken stoppingToken)
	{
		logger.LogInformation(
			"NLP process worker started.");

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await using var scope =
					scopeFactory.CreateAsyncScope();

				var mediator = scope.ServiceProvider
					.GetRequiredService<IMediator>();

				var result = await mediator.Send(
					new NlpProcessCommand(),
					stoppingToken);

				if (result.IsFailure)
				{
					logger.LogError(
						"NLP processing failed: {Error}",
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
					"Unexpected error while processing NLP.");
			}

			try
			{
				await Task.Delay(
					Interval,
					stoppingToken);
			}
			catch (OperationCanceledException)
				when (stoppingToken.IsCancellationRequested)
			{
				break;
			}
		}

		logger.LogInformation(
			"NLP process worker stopped.");
	}
}