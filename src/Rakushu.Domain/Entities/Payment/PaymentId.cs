using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities.Payment;

public class PaymentId : StronglyTypedId<Guid>
{
	public PaymentId(Guid value) : base(value)
	{
	}

	public static PaymentId Create() => new(Guid.NewGuid());

	public static PaymentId From(Guid value) => new(value);
}
