using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Subscription.Payment.GetPaymentById;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Subscription.Payment.GetPayments;

public sealed record GetPaymentsQuery(
	int PageNumber,
	int PageSize,
	Guid? UserId = null,
	Guid? PlanId = null,
	string? Status = null
) : IRequest<Result<PaginatedList<PaymentDto>>>;
