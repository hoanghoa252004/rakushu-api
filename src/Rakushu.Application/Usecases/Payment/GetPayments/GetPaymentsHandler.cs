using MediatR;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Payment.GetPayments;

public sealed class GetPaymentsHandler : IRequestHandler<GetPaymentsQuery, Result<PaginatedList<PaymentAdminDto>>>
{
	private readonly IPaymentQuery _paymentQuery;

	public GetPaymentsHandler(IPaymentQuery paymentQuery)
	{
		_paymentQuery = paymentQuery;
	}

	public async Task<Result<PaginatedList<PaymentAdminDto>>> Handle(GetPaymentsQuery request, CancellationToken cancellationToken)
	{
		var (items, totalCount) = await _paymentQuery.GetPaymentsAsync(request, cancellationToken);

		var result = PaginatedList<PaymentAdminDto>.Create(
			items,
			totalCount,
			request.PageNumber,
			request.PageSize);

		return Result.Success(result);
	}
}
