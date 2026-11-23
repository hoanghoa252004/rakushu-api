using FluentValidation;

namespace Rakushu.Application.Usecases.Analytics.Curator.GetCuratorHighErrorQuestions;

internal sealed class GetCuratorHighErrorQuestionsValidator
	: AbstractValidator<GetCuratorHighErrorQuestionsQuery>
{
	public GetCuratorHighErrorQuestionsValidator()
	{
		RuleFor(x => x.Days)
			.InclusiveBetween(1, 365)
			.WithMessage("Days must be between 1 and 365.");

		RuleFor(x => x.MinAnswers)
			.InclusiveBetween(1, 1000)
			.WithMessage("MinAnswers must be between 1 and 1000.");

		RuleFor(x => x.Page)
			.GreaterThanOrEqualTo(1)
			.WithMessage("Page must be at least 1.");

		RuleFor(x => x.PageSize)
			.InclusiveBetween(1, 100)
			.WithMessage("PageSize must be between 1 and 100.");
	}
}
