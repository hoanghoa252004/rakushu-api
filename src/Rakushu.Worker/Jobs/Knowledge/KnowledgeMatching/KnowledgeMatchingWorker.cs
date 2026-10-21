using MediatR;
using Rakushu.Application.Usecases.Knowledge.KnowledgeMatching.ProcessPendingVideoKnowledgeMatching;
using Rakushu.Application.Usecases.Material.Nlp.KnowledgeNlpProcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Worker.Jobs.Knowledge.KnowledgeMatching;
public sealed class KnowledgeMatchingWorker(
	IServiceScopeFactory scopeFactory,
	ILogger<KnowledgeMatchingWorker> logger) : BackgroundService
{
	private static readonly TimeSpan Interval =
		TimeSpan.FromSeconds(10);

	protected override async Task ExecuteAsync(
		CancellationToken stoppingToken)
	{
		logger.LogInformation(
			"KnowledgeMatchingWorker process worker started.");

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await using var scope =
					scopeFactory.CreateAsyncScope();

				var mediator = scope.ServiceProvider
					.GetRequiredService<IMediator>();

				var result = await mediator.Send(
					new ProcessPendingVideoKnowledgeMatchingCommand(),
					stoppingToken);

				if (result.IsFailure)
				{
					logger.LogError(
						"KnowledgeMatchingWorker processing failed: {Error}",
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
					"Unexpected error while processing KnowledgeMatchingWorker.");
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
			"KnowledgeMatchingWorker stopped.");
	}
}