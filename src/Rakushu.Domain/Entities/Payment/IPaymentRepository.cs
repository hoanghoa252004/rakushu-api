using Rakushu.Domain.Common.Contract;

namespace Rakushu.Domain.Entities.Payment;

public interface IPaymentRepository : IBaseRepository<Payment, PaymentId>
{
	Task<Payment?> GetByOrderCodeAsync(string orderCode, CancellationToken cancellationToken = default);
	Task<Payment?> GetByIdWithTransactionsAsync(PaymentId id, CancellationToken cancellationToken = default);
	Task<bool> HasTransactionWithSepayIdAsync(long sepayId, CancellationToken cancellationToken = default);
	Task<List<Payment>> GetExpiredPendingPaymentsAsync(DateTimeOffset now, CancellationToken cancellationToken = default);
}
