using Amazon.SimpleEmail;
using Microsoft.Extensions.Options;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Abstractions.Infrastructure.Payment;
using Rakushu.Infrastructure.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Infrastructure.Payment;

internal sealed class VnPayService : IPaymentService
{
	private readonly VnPaySettings _vnPaySettings;
	private readonly ISystemClock _systemClock;

	public VnPayService(
		IOptions<VnPaySettings> options,
		ISystemClock systemClock
		)
	{ 
		_vnPaySettings = options.Value;
		_systemClock = systemClock;
	}

	public DateTimeOffset GetPaymentExpiration()
		=> _systemClock.UtcNow.AddSeconds(_vnPaySettings.PaymentExpiredInSeconds);

	public DateTimeOffset GetTransactionExpiration()
		=> _systemClock.UtcNow.AddSeconds(_vnPaySettings.TransactionExpiredInSeconds);

}
