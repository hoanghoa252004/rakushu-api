using MediatR;
using Rakushu.Application.Abstractions.Persistence.Queries;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Payment.Payment.GetPaymentById;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Payment.Payment.GetPayments;

public sealed record GetPaymentsQuery(
	int PageNumber,
	int PageSize,
	Guid? UserId = null,
	Guid? PlanId = null,
	string? Status = null
) : IRequest<Result<PaginatedList<PaymentDto>>>;
