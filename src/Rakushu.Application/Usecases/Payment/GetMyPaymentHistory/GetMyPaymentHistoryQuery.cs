using MediatR;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Payment.GetMyPaymentHistory;

public sealed record GetMyPaymentHistoryQuery(
	int PageNumber,
	int PageSize,
	Guid? PlanId = null,
	string? Status = null
) : IRequest<Result<PaginatedList<PaymentDto>>>;
