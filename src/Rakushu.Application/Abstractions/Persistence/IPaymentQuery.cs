using Rakushu.Application.Usecases.Payment.GetMyPayments;
using Rakushu.Application.Usecases.Payment.GetPaymentById;
using Rakushu.Application.Usecases.Payment.GetPayments;
using Rakushu.Domain.Entities.Payment;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Application.Abstractions.Persistence;

public interface IPaymentQuery
{
	Task<PaymentDto?> GetByIdAsync(PaymentId id, CancellationToken cancellationToken = default);

	Task<(IReadOnlyCollection<PaymentSummaryDto> Items, int TotalCount)> GetMyPaymentsAsync(
		UserId userId,
		int pageNumber,
		int pageSize,
		CancellationToken cancellationToken = default);

	Task<(IReadOnlyCollection<PaymentAdminDto> Items, int TotalCount)> GetPaymentsAsync(
		GetPaymentsQuery query,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyCollection<PaymentTransactionDto>> GetTransactionsByPaymentIdAsync(
		PaymentId paymentId,
		CancellationToken cancellationToken = default);
}
