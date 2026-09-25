using FluentValidation;
using Rakushu.Application.Usecases.Transaction.CreateTransaction;

namespace Rakushu.Application.Usecases.Transaction.CreateTransaction;

public sealed class CreateTransactionValidator : AbstractValidator<CreateTransactionCommand>
{
	public CreateTransactionValidator()
	{
		RuleFor(x => x.PaymentId)
			.NotEmpty()
			.WithMessage("Payment ID is required");

		RuleFor(x => x.Provider)
			.NotEmpty()
			.WithMessage("Provider is required")
			.Matches("^(VNPAY|SEPAY)$")
			.WithMessage("Provider must be either VNPAY or SEPAY");
	}
}
