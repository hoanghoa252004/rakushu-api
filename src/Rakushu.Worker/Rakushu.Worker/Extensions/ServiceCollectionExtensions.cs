using Rakushu.Worker.Workers.Payment;
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
		services.AddHostedService<PaymentExpirationWorker>();

		services.AddHostedService<TransactionExpiryWorker>();

		return services;
	}
}
