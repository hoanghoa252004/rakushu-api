using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Srs.BankQuestion.GetBankQuestionReviewQueue;

internal sealed class GetBankQuestionReviewQueueValidator
	: AbstractValidator<GetBankQuestionReviewQueueQuery>
{
	public GetBankQuestionReviewQueueValidator()
	{
		RuleFor(x => x.Page)
			.GreaterThanOrEqualTo(1)
			.WithMessage("Page must be at least 1.");

		RuleFor(x => x.PageSize)
			.InclusiveBetween(1, 100)
			.WithMessage("PageSize must be between 1 and 100.");
	}
}
