using MediatR;
using Rakushu.Application.Usecases.Material.Nlp.KnowledgeNlpProcess;
using Rakushu.Application.Usecases.Material.Nlp.NlpProcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Worker.Jobs.Knowledge.KnowledgeNlp;

public sealed class KnowledgeNlpProcessWorker(
	IServiceScopeFactory scopeFactory,
	ILogger<KnowledgeNlpProcessWorker> logger) : BackgroundService
{
	private static readonly TimeSpan Interval =
		TimeSpan.FromSeconds(10);

	protected override async Task ExecuteAsync(
		CancellationToken stoppingToken)
	{
		logger.LogInformation(
			"KnowledgeNlpProcessWorker process worker started.");

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await using var scope =
					scopeFactory.CreateAsyncScope();

				var mediator = scope.ServiceProvider
					.GetRequiredService<IMediator>();

				var result = await mediator.Send(
					new KnowledgeNlpProcessCommand(),
					stoppingToken);

				if (result.IsFailure)
				{
					logger.LogError(
						"KnowledgeNlpProcessWorker processing failed: {Error}",
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
					"Unexpected error while processing KnowledgeNlpProcessWorker.");
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
			"KnowledgeNlpProcessWorker stopped.");
	}
}