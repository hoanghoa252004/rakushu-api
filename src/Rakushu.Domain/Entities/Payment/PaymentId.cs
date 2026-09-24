using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Payment;

public class PaymentId : StronglyTypedId<Guid>
{
	private PaymentId(Guid value) : base(value)
	{
	}

	public static PaymentId Create() => new(Guid.NewGuid());

	public static PaymentId From(Guid value) => new(value);
}
