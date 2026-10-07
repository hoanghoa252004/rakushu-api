using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Payment.Payment.GetPaymentById;
using Rakushu.Application.Usecases.Payment.Transaction.GetTransactionsByPaymentId;
using Rakushu.Application.Usecases.Subscription.Plan.GetPlanById;
using Rakushu.Application.Usecases.Subscription.Subscription.GetSubscriptions;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Payment;

namespace Rakushu.Application.Usecases.Payment.Payment.GetPayments;

public sealed class GetPaymentsHandler : IRequestHandler<GetPaymentsQuery, Result<PaginatedList<PaymentDto>>>
{
	private readonly IPaymentRepository _paymentRepository;

	public GetPaymentsHandler(IPaymentRepository paymentRepository)
	{
		_paymentRepository = paymentRepository;
	}

	public async Task<Result<PaginatedList<PaymentDto>>> Handle(GetPaymentsQuery request, CancellationToken cancellationToken)
	{
		// Validate status if provided
		if (!string.IsNullOrWhiteSpace(request.Status) && !Enum.TryParse<PaymentStatus>(request.Status, true, out _))
		{
			return Result.Failure<PaginatedList<PaymentDto>>(PaymentError.InvalidStatus);
		}

		var list = await _paymentRepository.GetAllAsync(cancellationToken);


		if (request.UserId.HasValue)
		{
			list = list.Where(p => p.UserId.Value == request.UserId);
		}

		if (request.PlanId.HasValue)
		{
			list = list.Where(p => p.PlanId.Value == request.PlanId);
		}

		if (!string.IsNullOrWhiteSpace(request.Status))
		{
			list = list.Where(p => p.Status.ToString().ToLower() == request.Status.ToLower());
		}

		// Calculate total count before paging
		var totalCount = list.Count();

		// Apply paging
		var items = list
			.OrderBy(c => c.UpdatedAt)
			.Skip((request.PageNumber - 1) * request.PageSize)
			.Take(request.PageSize)
			.Select(payment => new PaymentDto(
				payment.Id.Value,
				new UserBasicDto(
					payment.UserId.Value,
					payment.User.FullName,
					payment.User.Email
				),
				new PlanBasicDto(
					payment.PlanId.Value,
					payment.Plan.Code,
					payment.Plan.Name,
					payment.Plan.JapaneseName,
					payment.Plan.Price,
					payment.Plan.Currency.ToString(),
					payment.Plan.BillingCycle.ToString(),
					payment.Plan.IsActive,
					payment.Plan.Description
				),
				payment.Amount,
				payment.Currency.ToString(),
				payment.Status.ToString(),
				payment.ExpiredAt,
				payment.CreatedAt,
				payment.UpdatedAt,
				payment.Transactions.Select(t => new TransactionDto(
					t.Id.Value,
					t.PaymentId.Value,
					t.Provider.ToString(),
					t.Amount,
					t.Currency.ToString(),
					t.TxnRef,
					t.Url,
					t.TransactionNo,
					t.RawResponsePayload,
					t.Status.ToString(),
					t.ExpiredAt,
					t.CreatedAt,
					t.UpdatedAt
				)).ToList()))
			.ToList();

		var paginatedList = new PaginatedList<PaymentDto>(
			items,
			totalCount,
			request.PageNumber,
			request.PageSize);

		return Result.Success(paginatedList);
	}
}
