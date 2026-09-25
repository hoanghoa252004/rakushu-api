using FluentValidation;
using Rakushu.Application.Usecases.Payment.CreatePayment;

namespace Rakushu.Application.Usecases.Payment.CreatePayment;

public sealed class CreatePaymentValidator : AbstractValidator<CreatePaymentCommand>
{
	public CreatePaymentValidator()
	{
		RuleFor(x => x.PlanId)
			.NotEmpty()
			.WithMessage("Plan ID is required");
	}
}
