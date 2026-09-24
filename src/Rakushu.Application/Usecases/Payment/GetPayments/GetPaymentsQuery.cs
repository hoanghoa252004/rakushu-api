using MediatR;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Payment.GetPayments;

public sealed record GetPaymentsQuery(
	int PageNumber,
	int PageSize,
	Guid? UserId = null,
	Guid? PlanId = null,
	string? Status = null
) : IRequest<Result<PaginatedList<PaymentDto>>>;
