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
		var userId = _currentUserContext.UserId;
		if (userId == null)
		{
			return Result.Failure<PaymentDto>(UserError.NotFound);
		}

		var payment = await _paymentQuery.GetByIdAsync(PaymentId.From(request.PaymentId), cancellationToken);
		if (payment == null)
		{
			return Result.Failure<PaymentDto>(PaymentErrors.NotFound);
		}

		var isAdmin = _currentUserContext.Role == DefaultSystemRoles.SystemAdministrator.ToString();
		if (!isAdmin && payment.UserId != userId.Value)
		{
			return Result.Failure<PaymentDto>(UserError.UnauthorizedResourceAccess);
		}

		return Result.Success(payment);
	}
}
