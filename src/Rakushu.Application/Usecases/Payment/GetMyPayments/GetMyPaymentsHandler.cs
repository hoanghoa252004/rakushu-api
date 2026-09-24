using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Application.Usecases.Payment.GetMyPayments;

public sealed class GetMyPaymentsHandler : IRequestHandler<GetMyPaymentsQuery, Result<PaginatedList<PaymentSummaryDto>>>
{
	private readonly ICurrentUserContext _currentUserContext;
	private readonly IPaymentQuery _paymentQuery;

	public GetMyPaymentsHandler(
		ICurrentUserContext currentUserContext,
		IPaymentQuery paymentQuery)
	{
		_currentUserContext = currentUserContext;
		_paymentQuery = paymentQuery;
	}

	public async Task<Result<PaginatedList<PaymentSummaryDto>>> Handle(GetMyPaymentsQuery request, CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId == null)
		{
			return Result.Failure<PaginatedList<PaymentSummaryDto>>(UserError.NotFound);
		}

		var (items, totalCount) = await _paymentQuery.GetMyPaymentsAsync(
			userId,
			request.PageNumber,
			request.PageSize,
			cancellationToken);

		var result = PaginatedList<PaymentSummaryDto>.Create(
			items,
			totalCount,
			request.PageNumber,
			request.PageSize);

		return Result.Success(result);
	}
}
