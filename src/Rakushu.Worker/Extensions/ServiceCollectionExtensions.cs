using Rakushu.Worker.Jobs.Embedding;
using Rakushu.Worker.Jobs.Knowledge.KnowledgeMatching;
using Rakushu.Worker.Jobs.Knowledge.KnowledgeNlp;
using Rakushu.Worker.Jobs.Payment;
using Rakushu.Worker.Jobs.Subscription;
using Rakushu.Worker.Jobs.Video.Nlp;
using Rakushu.Worker.Jobs.Video.Transcript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Worker.Extensions;

internal static class ServiceCollectionExtensions
{
	internal static IServiceCollection AddRakushuWorkerServices(this IServiceCollection services)
	{
		// PAYMENT
		//services.AddHostedService<PaymentExpirationWorker>();
		//services.AddHostedService<TransactionExpiryWorker>();
		//services.AddHostedService<TranscriptImportWorker>();
		//services.AddHostedService<NlpProcessWorker>();
		//services.AddHostedService<KnowledgeNlpProcessWorker>();
		services.AddHostedService<KnowledgeMatchingWorker>();

		// SUBSCRIPTION
		//services.AddHostedService<SubscriptionTrackingWorker>();

		return services;
	}
}