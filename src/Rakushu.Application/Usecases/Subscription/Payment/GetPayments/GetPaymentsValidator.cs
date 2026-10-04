using FluentValidation;

namespace Rakushu.Application.Usecases.Subscription.Payment.GetPayments;

public sealed class GetPaymentsValidator : AbstractValidator<GetPaymentsQuery>
{
	public GetPaymentsValidator()
	{
		RuleFor(x => x.PageNumber)
			.GreaterThan(0)
			.WithMessage("Page number must be greater than 0");

		RuleFor(x => x.PageSize)
			.GreaterThan(0)
			.LessThanOrEqualTo(100)
			.WithMessage("Page size must be between 1 and 100");
	}
}
