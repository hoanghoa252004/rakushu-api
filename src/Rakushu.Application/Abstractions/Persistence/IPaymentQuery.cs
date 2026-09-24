using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Payment.GetMyPaymentHistory;
using Rakushu.Application.Usecases.Payment.GetPayments;
using Rakushu.Domain.Entities.Payment;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Application.Abstractions.Persistence;

public interface IPaymentQuery
{
	Task<PaymentDto?> GetPaymentByIdAsync(
		PaymentId paymentId,
		CancellationToken cancellationToken = default);

	Task<(IReadOnlyCollection<PaymentDto> Items, int TotalCount)> GetPaymentsAsync(
		GetPaymentsQuery query,
		CancellationToken cancellationToken = default);

	Task<(IReadOnlyCollection<PaymentDto> Items, int TotalCount)> GetMyPaymentsAsync(
		UserId userId,
		GetMyPaymentHistoryQuery query,
		CancellationToken cancellationToken = default);
}

public sealed record PaymentDto(
	Guid Id,
	Guid? UserId,
	Guid PlanId,
	decimal Amount,
	string Currency,
	string Status,
	DateTime ExpiredAt,
	DateTime CreatedAt,
	DateTime UpdatedAt
);
