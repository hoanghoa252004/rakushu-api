using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Payment.GetMyPayments;

public sealed record GetMyPaymentsQuery(
	int PageNumber = 1,
	int PageSize = 10
) : IRequest<Result<PaginatedList<PaymentSummaryDto>>>;
