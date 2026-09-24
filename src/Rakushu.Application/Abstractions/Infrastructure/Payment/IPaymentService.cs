using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Abstractions.Infrastructure.Payment;

public interface IPaymentService
{
	DateTimeOffset GetPaymentExpiration();
	DateTimeOffset GetTransactionExpiration();
}
