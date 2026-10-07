using FluentValidation;

namespace Rakushu.Application.Usecases.Payment.Payment.CreatePayment;

public sealed class CreatePaymentValidator : AbstractValidator<CreatePaymentCommand>
{
	public CreatePaymentValidator()
	{
		RuleFor(x => x.PlanId)
			.NotEmpty()
			.WithMessage("Plan ID is required");
	}
}
