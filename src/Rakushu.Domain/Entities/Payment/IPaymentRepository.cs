using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Payment;

namespace Rakushu.Domain.Entities.Payment;

public interface IPaymentRepository : IBaseRepository<Payment, PaymentId>
{
}
