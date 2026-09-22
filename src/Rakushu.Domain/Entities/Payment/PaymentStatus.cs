namespace Rakushu.Domain.Entities.Payment;

public enum PaymentStatus
{
	Pending = 0,
	Completed = 1,
	Failed = 2,
	Canceled = 3,
	Expired = 4
}
