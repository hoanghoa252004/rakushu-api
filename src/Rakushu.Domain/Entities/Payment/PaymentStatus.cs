using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Payment;

public enum PaymentStatus
{
	Pending = 0,
	Completed = 1,
	Expired = 2,
	Failed = 3,
	Cancelled = 4
}
