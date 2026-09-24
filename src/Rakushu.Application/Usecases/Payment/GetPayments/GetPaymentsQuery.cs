using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Payment.GetPayments;

public sealed record GetPaymentsQuery(
	int PageNumber = 1,
	int PageSize = 10,
	string? Status = null,
	string? OrderCode = null,
	Guid? UserId = null,
	Guid? PlanId = null
) : IRequest<Result<PaginatedList<PaymentAdminDto>>>;
