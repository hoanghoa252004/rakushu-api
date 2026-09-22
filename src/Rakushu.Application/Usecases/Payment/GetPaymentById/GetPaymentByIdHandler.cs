using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Payment;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Application.Usecases.Payment.GetPaymentById;

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
		var userId = _currentUserContext.UserId;
		if (userId == null)
		{
			return Result.Failure<PaymentDto>(UserError.NotFound);
		}

		var payment = await _paymentRepository.GetByIdWithTransactionsAsync(PaymentId.From(request.PaymentId), cancellationToken);
		if (payment == null)
		{
			return Result.Failure<PaymentDto>(PaymentErrors.NotFound);
		}

		if (payment.UserId != userId)
		{
			return Result.Failure<PaymentDto>(UserError.UnauthorizedResourceAccess);
		}

		var transactionsDto = payment.Transactions
			.OrderByDescending(t => t.CreatedAt)
			.Select(t => new PaymentTransactionDto(
				t.Id.Value,
				t.SepayId,
				t.Gateway,
				t.AccountNumber,
				t.TransactionDate,
				t.Content,
				t.TransferAmount,
				t.ReferenceCode,
				t.Status.ToString(),
				t.CreatedAt
			))
			.ToList();

		var dto = new PaymentDto(
			payment.Id.Value,
			payment.PlanId.Value,
			payment.Plan?.Name ?? string.Empty,
			payment.SubscriptionId?.Value,
			payment.OrderCode,
			payment.Amount,
			payment.Currency,
			payment.Status.ToString(),
			payment.Description,
			payment.QrCodeUrl,
			payment.ExpiresAt,
			payment.CompletedAt,
			payment.CreatedAt,
			transactionsDto
		);

		return Result.Success(dto);
	}
}
