using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities.Payment.PaymentTransaction;

public class PaymentTransactionId : StronglyTypedId<Guid>
{
	public PaymentTransactionId(Guid value) : base(value)
	{
	}

	public static PaymentTransactionId Create() => new(Guid.NewGuid());

	public static PaymentTransactionId From(Guid value) => new(value);
}
