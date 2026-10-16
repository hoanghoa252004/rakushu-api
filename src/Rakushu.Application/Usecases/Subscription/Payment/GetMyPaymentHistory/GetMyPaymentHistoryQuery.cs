using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Subscription.Payment.GetPaymentById;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Subscription.Payment.GetMyPaymentHistory;

public sealed record GetMyPaymentHistoryQuery(
	int PageNumber,
	int PageSize,
	Guid? PlanId = null,
	string? Status = null
) : IRequest<Result<PaginatedList<PaymentDto>>>;
