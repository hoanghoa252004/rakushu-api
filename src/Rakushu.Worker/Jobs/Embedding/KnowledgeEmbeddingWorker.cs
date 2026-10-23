using MediatR;
using Rakushu.Application.Usecases.Embedding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Worker.Jobs.Embedding;


public sealed class KnowledgeEmbeddingWorker(
	IServiceScopeFactory scopeFactory,
	ILogger<KnowledgeEmbeddingWorker> logger)
	: BackgroundService
{
	private static readonly TimeSpan Interval =
		TimeSpan.FromSeconds(10);

	protected override async Task ExecuteAsync(
		CancellationToken stoppingToken)
	{
		logger.LogInformation(
			"Knowledge embedding worker started.");

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await using var scope =
					scopeFactory.CreateAsyncScope();

				var mediator = scope.ServiceProvider
					.GetRequiredService<IMediator>();

				await mediator.Send(
					new KnowledgeEmbeddingCommand(),
					stoppingToken);
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
					"Unexpected error while embedding Knowledge.");
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
			"Knowledge embedding worker stopped.");
	}
}