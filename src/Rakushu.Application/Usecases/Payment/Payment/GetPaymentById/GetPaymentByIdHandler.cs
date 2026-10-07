using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Usecases.Payment.Transaction.GetTransactionsByPaymentId;
using Rakushu.Application.Usecases.Subscription.Plan.GetPlanById;
using Rakushu.Application.Usecases.Subscription.Subscription.GetSubscriptions;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Payment;
using Rakushu.Domain.Entities.Role;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.User.Subscription;

namespace Rakushu.Application.Usecases.Payment.Payment.GetPaymentById;

public sealed class GetPaymentByIdHandler : IRequestHandler<GetPaymentByIdQuery, Result<PaymentDto>>
{
	private readonly ICurrentUserContext _currentUserContext;
	private readonly IPaymentRepository _paymentRepository;

	public GetPaymentByIdHandler(
		ICurrentUserContext currentUserContext,
		IPaymentRepository paymentRepository)
	{
		_currentUserContext = currentUserContext;
		_paymentRepository = paymentRepository;
	}

	public async Task<Result<PaymentDto>> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
	{
		var paymentId = PaymentId.From(request.PaymentId);

		var payment = await _paymentRepository.GetByIdAsync(paymentId, cancellationToken);

		if (payment == null)
		{
			return Result.Failure<PaymentDto>(PaymentError.PaymentNotFound);
		}

		var paymentDto = new PaymentDto(
			payment.Id.Value,
			new UserBasicDto(
					payment.User.Id.Value,
					payment.User.FullName,
					payment.User.Email),
			new PlanBasicDto(
					payment.Plan.Id.Value,
					payment.Plan.Code,
					payment.Plan.Name,
					payment.Plan.JapaneseName,
					payment.Plan.Price,
					payment.Plan.Currency.ToString(),
					payment.Plan.BillingCycle.ToString(),
					payment.Plan.IsActive,
					payment.Plan.Description),
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
			)).ToList());

		// Check authorization
		var currentUserId = _currentUserContext.UserId;

		var currentRole = _currentUserContext.Role;

		// If learner, check if payment belongs to current user
		if (currentRole != RoleCodes.SystemAdministrator)
		{
			if (currentUserId == null || UserId.From(payment.UserId!.Value) != currentUserId)
			{
				return Result.Failure<PaymentDto>(PaymentError.PaymentNotBelong);
			}
		}

		var isAdmin = currentRole == RoleCodes.SystemAdministrator;

		if (!isAdmin)
		{
			paymentDto = paymentDto with
			{
				User = null
			};
		}

		return Result.Success(paymentDto);
	}
}
