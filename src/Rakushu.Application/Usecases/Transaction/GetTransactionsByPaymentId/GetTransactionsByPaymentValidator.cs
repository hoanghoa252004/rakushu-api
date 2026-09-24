using FluentValidation;
using Rakushu.Application.Usecases.Transaction.GetTransactionsByPayment;

namespace Rakushu.Application.Usecases.Transaction.GetTransactionsByPayment;

public sealed class GetTransactionsByPaymentValidator : AbstractValidator<GetTransactionsByPaymentIdQuery>
{
	public GetTransactionsByPaymentValidator()
	{
		RuleFor(x => x.PaymentId)
			.NotEmpty()
			.WithMessage("Payment ID is required");
	}
}
