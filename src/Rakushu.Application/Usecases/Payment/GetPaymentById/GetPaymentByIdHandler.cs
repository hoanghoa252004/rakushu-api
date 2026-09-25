using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Payment;
using Rakushu.Domain.Entities.Role;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Application.Usecases.Payment.GetPaymentById;

public sealed class GetPaymentByIdHandler : IRequestHandler<GetPaymentByIdQuery, Result<PaymentDto>>
{
	private readonly ICurrentUserContext _currentUserContext;
	private readonly IPaymentQuery _paymentQuery;

	public GetPaymentByIdHandler(
		ICurrentUserContext currentUserContext,
		IPaymentQuery paymentQuery)
	{
		_currentUserContext = currentUserContext;
		_paymentQuery = paymentQuery;
	}

	public async Task<Result<PaymentDto>> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
	{
		var paymentId = PaymentId.From(request.PaymentId);

		var payment = await _paymentQuery.GetPaymentByIdAsync(paymentId, cancellationToken);

		if (payment == null)
		{
			return Result.Failure<PaymentDto>(PaymentError.PaymentNotFound);
		}

		// Check authorization
		var currentUserId = _currentUserContext.UserId;

		var currentRole = _currentUserContext.Role;

		// If learner, check if payment belongs to current user
		if (currentRole != DefaultSystemRoles.SystemAdministrator.ToString())
		{
			if (currentUserId == null || UserId.From(payment.UserId!.Value) != currentUserId)
			{
				return Result.Failure<PaymentDto>(PaymentError.PaymentNotBelong);
			}
		}

		var isAdmin = currentRole == DefaultSystemRoles.SystemAdministrator.ToString();

		if (!isAdmin)
		{
			payment = payment with
			{
				UserId = null
			};
		}

		return Result.Success(payment);
	}
}
