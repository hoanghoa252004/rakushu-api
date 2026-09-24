using MediatR;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Payment;

namespace Rakushu.Application.Usecases.Payment.GetPayments;

public sealed class GetPaymentsHandler : IRequestHandler<GetPaymentsQuery, Result<PaginatedList<PaymentDto>>>
{
	private readonly IPaymentQuery _paymentQuery;

	public GetPaymentsHandler(IPaymentQuery paymentQuery)
	{
		_paymentQuery = paymentQuery;
	}

	public async Task<Result<PaginatedList<PaymentDto>>> Handle(GetPaymentsQuery request, CancellationToken cancellationToken)
	{
		// Validate status if provided
		if (!string.IsNullOrWhiteSpace(request.Status) && !Enum.TryParse<PaymentStatus>(request.Status, true, out _))
		{
			return Result.Failure<PaginatedList<PaymentDto>>(PaymentError.InvalidStatus);
		}

		var (items, totalCount) = await _paymentQuery.GetPaymentsAsync(request,cancellationToken);

		var paginatedList = new PaginatedList<PaymentDto>(
			items.ToList(),
			totalCount,
			request.PageNumber,
			request.PageSize);

		return Result.Success(paginatedList);
	}
}
