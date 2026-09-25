using FluentValidation;

namespace Rakushu.Application.Usecases.Transaction.GetTransactionsByPaymentId;

public sealed class GetTransactionsByPaymentValidator : AbstractValidator<GetTransactionsByPaymentIdQuery>
{
	public GetTransactionsByPaymentValidator()
	{
		RuleFor(x => x.PaymentId)
			.NotEmpty()
			.WithMessage("Payment ID is required");
	}
}
