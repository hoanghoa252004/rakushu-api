using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Payment.Transaction;

public enum TransactionStatus
{
	Pending = 0,
	Successful = 1,
	Cancelled = 2,
	Failed = 3,
	Expired = 4
}

