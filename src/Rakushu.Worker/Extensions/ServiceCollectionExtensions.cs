using Rakushu.Worker.Jobs.Payment;
using Rakushu.Worker.Jobs.Subscription;
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
		services.AddHostedService<PaymentExpirationWorker>();
		services.AddHostedService<TransactionExpiryWorker>();

		// SUBSCRIPTION
		services.AddHostedService<SubscriptionTracking>();

		return services;
	}
}