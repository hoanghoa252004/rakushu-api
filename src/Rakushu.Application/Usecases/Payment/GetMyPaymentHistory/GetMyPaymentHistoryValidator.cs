using FluentValidation;
using Rakushu.Application.Usecases.Payment.GetMyPaymentHistory;

namespace Rakushu.Application.Usecases.Payment.GetMyPaymentHistory;

public sealed class GetMyPaymentHistoryValidator : AbstractValidator<GetMyPaymentHistoryQuery>
{
	public GetMyPaymentHistoryValidator()
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
