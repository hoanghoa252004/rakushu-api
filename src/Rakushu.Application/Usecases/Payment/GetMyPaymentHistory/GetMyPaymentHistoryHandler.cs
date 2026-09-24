using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Payment;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Application.Usecases.Payment.GetMyPaymentHistory;

public sealed class GetMyPaymentHistoryHandler : IRequestHandler<GetMyPaymentHistoryQuery, Result<PaginatedList<PaymentDto>>>
{
	private readonly ICurrentUserContext _currentUserContext;
	private readonly IPaymentQuery _paymentQuery;

	public GetMyPaymentHistoryHandler(
		ICurrentUserContext currentUserContext,
		IPaymentQuery paymentQuery)
	{
		_currentUserContext = currentUserContext;
		_paymentQuery = paymentQuery;
	}

	public async Task<Result<PaginatedList<PaymentDto>>> Handle(GetMyPaymentHistoryQuery request, CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;

		if (userId == null)
		{
			return Result.Failure<PaginatedList<PaymentDto>>(PaymentError.InvalidUserId);
		}

		// Validate status if provided
		if (!string.IsNullOrWhiteSpace(request.Status) && !Enum.TryParse<PaymentStatus>(request.Status, true, out _))
		{
			return Result.Failure<PaginatedList<PaymentDto>>(PaymentError.InvalidStatus);
		}

		var (items, totalCount) = await _paymentQuery.GetMyPaymentsAsync(
			userId, request, cancellationToken);

		var currentRole = _currentUserContext.Role;

		var isAdmin = currentRole == DefaultSystemRoles.SystemAdministrator.ToString();

		if (!isAdmin)
		{
			items = items.Select(t => t with { UserId = null }).ToList();
		}

		var paginatedList = new PaginatedList<PaymentDto>(
			items.ToList(),
			totalCount,
			request.PageNumber,
			request.PageSize);

		return Result.Success(paginatedList);
	}
}
