using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Payment.Transaction;

public class TransactionId : StronglyTypedId<Guid>
{
	private TransactionId(Guid value) : base(value)
	{
	}

	public static TransactionId Create() => new(Guid.NewGuid());

	public static TransactionId From(Guid value) => new(value);
}